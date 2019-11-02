using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class StockQuantService : BaseService<StockQuant>, IStockQuantService
    {

        public StockQuantService(IAsyncRepository<StockQuant> repository, IHttpContextAccessor httpContextAccessor)
        : base(repository, httpContextAccessor)
        {
        }

        public async Task<IList<QuantOrder>> QuantsGetReservation(decimal qty, StockMove move)
        {
            var reservations = new List<QuantOrder>() { new QuantOrder { Quant = null, Qty = qty } };
            var location = move.Location;

            var states = new string[] { "inventory", "production", "supplier" };
            if (states.Contains(location.Usage))
                return reservations;

            return await _QuantsGetReservation(qty, move);
        }

        private async Task<IList<QuantOrder>> _QuantsGetReservation(decimal quantity, StockMove move)
        {
            Func<IQueryable<StockQuant>, IOrderedQueryable<StockQuant>> order = x => x.OrderBy(s => s.InDate).ThenBy(s => s.DateCreated);
            Expression<Func<StockQuant, bool>> domain = x => x.Qty > 0 && x.ProductId == move.ProductId &&
            x.CompanyId == move.CompanyId && ((x.Location.ParentLeft >= move.Location.ParentLeft && x.Location.ParentLeft <= move.Location.ParentRight) || x.LocationId == move.Location.Id);
            var res = new List<QuantOrder>();
            var offset = 0;

            var remaining_quantity = quantity;
            var quants = await SearchQuery(domain, orderBy: order, limit: 10, offSet: offset)
                .ToListAsync();
            while (remaining_quantity > 0 &&
                quants.Any())
            {
                foreach (var quant in quants)
                {
                    if (remaining_quantity >= quant.Qty)
                    {
                        res.Add(new QuantOrder { Quant = quant, Qty = Math.Abs(quant.Qty) });
                        remaining_quantity -= Math.Abs(quant.Qty);
                    }
                    else if (remaining_quantity != 0)
                    {
                        res.Add(new QuantOrder { Quant = quant, Qty = remaining_quantity });
                        remaining_quantity = 0;
                        break;
                    }
                }

                if (remaining_quantity == 0)
                    break;
                offset += 10;
                quants = await SearchQuery(domain, orderBy: order, limit: 10, offSet: offset)
                    .ToListAsync();
            }

            if (remaining_quantity > 0)
                res.Add(new QuantOrder { Quant = null, Qty = remaining_quantity });
            return res;
        }

        //Moves all given stock.quant in the given destination location.  Unreserve from current move.
        public async Task QuantsMove(IList<QuantOrder> quants, StockMove move, StockLocation locationDest,
            StockLocation locationFrom = null)
        {
            if (locationDest.Usage == "view")
                throw new Exception("Bạn không thể chuyển đến địa điểm kiểu khung nhìn.");
          
            var quantsReconcile = new List<StockQuant>().AsEnumerable();
            var toMoveQuants = new List<StockQuant>().AsEnumerable();

            foreach (var quantOrder in quants)
            {
                if (quantOrder.Quant == null)
                {
                    quantOrder.Quant = await QuantCreate(quantOrder.Qty, move, forceLocationFrom: locationFrom, forceLocationTo: locationDest);
                }
                else
                {
                    await QuantSplit(quantOrder.Quant, quantOrder.Qty);
                    toMoveQuants = toMoveQuants.Union(new List<StockQuant>() { quantOrder.Quant });
                }

                quantsReconcile = quantsReconcile.Union(new List<StockQuant>() { quantOrder.Quant });
            }

            //should be done
            if (toMoveQuants.Any())
            {
                await MoveQuantsWrite(toMoveQuants.ToList(), move, locationDest);
            }

            if (locationDest.Usage == "internal")
            {
                var negative = await SearchQuery(domain: x => x.ProductId == move.ProductId && x.Qty < 0 &&
                ((x.Location.ParentLeft >= locationDest.ParentLeft && x.Location.ParentLeft <= locationDest.ParentRight) ||
                x.LocationId == locationDest.Id)).FirstOrDefaultAsync();
                if (negative != null)
                {
                    foreach (var quant in quantsReconcile)
                        await _QuantReconcileNegative(quant, move);
                }
            }
        }

        public async Task _QuantReconcileNegative(StockQuant self, StockMove move)
        {
            var solvingQuant = self;
            var quants = await _SearchQuantsToReconcile(self);
            await EnsureLoadHistory(self);
            var history_ids = self.StockQuantMoveRels.Select(x => x.MoveId);
            foreach (var item in quants)
            {
                var quantNeg = item.Quant;
                var qty = item.Qty;
                if (quantNeg == null || solvingQuant == null)
                    continue;
                var toSolveQuants = await SearchQuery(x => x.PropagatedFromId == quantNeg.Id && !x.StockQuantMoveRels.Any(s => history_ids.Contains(s.MoveId))).ToListAsync();
                if (!toSolveQuants.Any())
                    continue;
                var solvingQty = qty;
                decimal qty_solved = 0;
                var solvedQuants = new List<StockQuant>().AsEnumerable();
                foreach (var toSolveQuant in toSolveQuants)
                {
                    if (solvingQty <= 0)
                        continue;
                    solvedQuants = solvedQuants.Union(new List<StockQuant>() { toSolveQuant });
                    await QuantSplit(toSolveQuant, Math.Min(solvingQty, toSolveQuant.Qty));
                    qty_solved += Math.Min(solvingQty, toSolveQuant.Qty);
                    solvingQty -= Math.Min(solvingQty, toSolveQuant.Qty);
                }

                var remainingSolvingQuant = await QuantSplit(solvingQuant, qty_solved);
                var remainingNegQuant = await QuantSplit(quantNeg, -qty_solved);
                if (remainingNegQuant != null)
                {
                    var solvedQuantIds = solvedQuants.Select(x => x.Id);
                    var remainingToSolveQuants = await SearchQuery(x => x.PropagatedFromId == quantNeg.Id && !solvedQuantIds.Contains(x.Id)).ToListAsync();
                    if (remainingToSolveQuants.Any())
                    {
                        foreach (var remainingToSolveQuant in remainingToSolveQuants)
                        {
                            remainingToSolveQuant.PropagatedFrom = remainingNegQuant;
                            remainingToSolveQuant.PropagatedFromId = remainingNegQuant.Id;
                        }
                        await UpdateAsync(remainingToSolveQuants);
                    }
                }
                if (solvingQuant.PropagatedFrom != null && solvedQuants.Any())
                {
                    foreach (var solvedQuant in solvedQuants)
                    {
                        solvedQuant.PropagatedFrom = solvingQuant.PropagatedFrom;
                        solvedQuant.PropagatedFromId = solvingQuant.PropagatedFromId;
                    }
                    await UpdateAsync(solvedQuants);
                }

                await DeleteAsync(quantNeg);

                if (solvedQuants.Any())
                {
                    await PriceUpdate(solvedQuants, solvingQuant.Cost);
                    await _QuantsMerge(solvedQuants, solvingQuant);
                }

                await DeleteAsync(solvingQuant);
                solvingQuant = remainingSolvingQuant;
            }
        }

        private async Task PriceUpdate(IEnumerable<StockQuant> quants, decimal newPrice)
        {
            foreach (var quant in quants)
            {
                quant.Cost = newPrice;
            }
            await UpdateAsync(quants);
        }

        private async Task _QuantsMerge(IEnumerable<StockQuant> solvedQuants, StockQuant solvingQuant)
        {
            foreach (var quant in solvedQuants)
            {
                await EnsureLoadHistory(quant);
                foreach(var rel in solvingQuant.StockQuantMoveRels)
                {
                    if (!quant.StockQuantMoveRels.Any(x => x.MoveId == rel.MoveId))
                    {
                        quant.StockQuantMoveRels.Add(new StockQuantMoveRel
                        {
                            MoveId = rel.MoveId
                        });
                    }
                }
            }

            await UpdateAsync(solvedQuants);
        }

        private async Task<IList<QuantOrder>> _SearchQuantsToReconcile(StockQuant self)
        {
            Expression<Func<StockQuant, bool>> dom = x => x.Qty < 0 &&
            x.Location.ParentLeft >= self.Location.ParentLeft && x.Location.ParentLeft < self.Location.ParentRight &&
            x.ProductId == self.ProductId &&
            x.Id != self.PropagatedFromId;

            var quantsSearch = await SearchQuery(dom, orderBy: x => x.OrderBy(s => s.InDate)).ToListAsync();
            var quants = new List<QuantOrder>();
            var quantity = self.Qty;
            foreach (var quantSearch in quantsSearch)
            {
                if (quantity >= Math.Abs(quantSearch.Qty))
                {
                    quants.Add(new QuantOrder { Quant = quantSearch, Qty = Math.Abs(quantSearch.Qty) });
                    quantity -= Math.Abs(quantSearch.Qty);
                }
                else if (quantity != 0)
                {
                    quants.Add(new QuantOrder { Quant = quantSearch, Qty = quantity });
                    quantity = 0;
                    break;
                }
            }
            return quants;
        }

        public async Task MoveQuantsWrite(IList<StockQuant> quants, StockMove move, StockLocation locationDest)
        {
            foreach (var quant in quants)
            {
                await EnsureLoadHistory(quant);
                quant.LocationId = locationDest.Id;
                quant.StockQuantMoveRels.Add(new StockQuantMoveRel { MoveId = move.Id });
            }

            await UpdateAsync(quants);
        }

        private async Task EnsureLoadHistory(StockQuant quant)
        {
            if (!GetEntry(quant).Collection(x => x.StockQuantMoveRels).IsLoaded)
                await GetEntry(quant).Collection(x => x.StockQuantMoveRels).LoadAsync();
        }

        public async Task<StockQuant> QuantSplit(StockQuant quant, decimal qty)
        {
            if (Math.Abs(quant.Qty) <= Math.Abs(qty))
                return null;

            var qtyRound = qty;
            var newQtyRound = quant.Qty - qty;

            var newQuant = new StockQuant(quant);
            newQuant.Qty = newQtyRound;
            await EnsureLoadHistory(quant);

            //newQuant.StockMoves = quant.StockMoves.ToList();
            foreach (var rel in quant.StockQuantMoveRels)
            {
                newQuant.StockQuantMoveRels.Add(new StockQuantMoveRel
                {
                    MoveId = rel.MoveId
                });
            }
            
            await CreateAsync(newQuant);

            quant.Qty = qtyRound;
            await UpdateAsync(quant);

            return newQuant;
        }

        public async Task<StockQuant> QuantCreate(decimal qty, StockMove move,
            StockLocation forceLocationFrom = null, StockLocation forceLocationTo = null)
        {
            var priceUnit = move.PriceUnit;
            var location = forceLocationTo != null ? forceLocationTo : move.LocationDest;

            var quant = new StockQuant()
            {
                ProductId = move.ProductId,
                LocationId = location.Id,
                Location = location,
                Qty = qty,
                Cost = priceUnit,
                InDate = DateTime.Now,
                CompanyId = location.CompanyId ?? move.CompanyId,
            };

            quant.StockQuantMoveRels.Add(new StockQuantMoveRel
            {
                MoveId = move.Id
            });

            //if we were trying to move something from an internal location and reach here (quant creation),
            //it means that a negative quant has to be created as well.
            if (move.Location.Usage == "internal")
            {
                var negativeQuant = new StockQuant(quant);
                negativeQuant.CompanyId = forceLocationFrom != null ? forceLocationFrom.CompanyId.Value : (move.Location.CompanyId ?? move.CompanyId);
                negativeQuant.LocationId = forceLocationFrom != null ? forceLocationFrom.Id : move.LocationId;
                negativeQuant.Location = forceLocationFrom != null ? forceLocationFrom : move.Location;
                negativeQuant.Qty = -negativeQuant.Qty;
                negativeQuant.Cost = priceUnit;
                negativeQuant.NegativeMoveId = move.Id;
                await CreateAsync(negativeQuant);

                quant.PropagatedFrom = negativeQuant;
            }

            await CreateAsync(quant);

            return quant;
        }

        public override ISpecification<StockQuant> RuleDomainGet(IRRule rule)
        {
            var companyId = CompanyId;
            switch (rule.Code)
            {
                case "stock.quant_comp_rule":
                    return new InitialSpecification<StockQuant>(x => x.CompanyId == companyId);
                default:
                    return null;
            }
        }
    }

    public class QuantOrder
    {
        public StockQuant Quant { get; set; }

        public decimal Qty { get; set; }
    }
}
