using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class IRSequenceService : BaseService<IRSequence>, IIRSequenceService
    {
        public IRSequenceService(IAsyncRepository<IRSequence> repository, IHttpContextAccessor httpContextAccessor)
           : base(repository, httpContextAccessor)
        {
        }

        public async Task<string> NextByCode(string code)
        {
            var sequence = await SearchQuery(domain: x => x.Code == code).FirstOrDefaultAsync();
            return await Next(sequence);
        }

        public async Task<string> Next(IRSequence sequence)
        {
            if (sequence == null)
                return null;

            var numberNext = sequence.NumberNext;
            sequence.NumberNext += sequence.NumberIncrement;

            await UpdateAsync(sequence);

            try
            {
                var now = DateTime.Now;
                var interpolatedPrefix = "";
                var interpolatedSuffix = "";
                var d = _InterpolationDict();
                if (!string.IsNullOrEmpty(sequence.Prefix))
                {
                    interpolatedPrefix = _Interpolate(sequence.Prefix, d);
                }

                if (!string.IsNullOrEmpty(sequence.Suffix))
                {
                    interpolatedSuffix = _Interpolate(sequence.Suffix, d);
                }

                return interpolatedPrefix + numberNext.ToString("D" + sequence.Padding) + interpolatedSuffix;
            }
            catch
            {
                throw new Exception("Invalid prefix or suffix for sequence");
            }
        }

        public static IDictionary<string, string> _InterpolationDict()
        {
            var t = DateTime.Now;
            var dict = new Dictionary<string, string>();
            dict.Add("{yy}", t.ToString("yy"));
            dict.Add("{yyyy}", t.ToString("yyyy"));
            dict.Add("{MM}", t.ToString("MM"));
            dict.Add("{dd}", t.ToString("dd"));
            return dict;
        }

        private static string _Interpolate(string s, IDictionary<string, string> d)
        {
            var regex = new Regex(@"\{\w+\}");
            var matches = regex.Matches(s);
            foreach (Match match in matches)
            {
                if (d.ContainsKey(match.Value))
                    s = s.Replace(match.Value, d[match.Value]);
            }
            return s;
        }

        public async Task<string> NextById(Guid id)
        {
            var sequence = await GetByIdAsync(id);
            return await Next(sequence);
        }
    }
}
