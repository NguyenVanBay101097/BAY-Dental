using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Models.ContentEditing;

namespace Infrastructure.Services
{
    public class SurveyUserInputService : BaseService<SurveyUserInput> , ISurveyUserInputService
    {
        readonly public IMapper _mapper;
        public SurveyUserInputService(IAsyncRepository<SurveyUserInput> repository, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
            : base(repository, httpContextAccessor)
        {
            _mapper = mapper;
        }

        public async Task<SurveyUserInputDisplay> GetDisplay(Guid id)
        {
            var res = await _mapper.ProjectTo<SurveyUserInputDisplay>(SearchQuery(x => x.Id == id)).FirstOrDefaultAsync();
            if (res == null)
                throw new NullReferenceException("Khảo sát không tồn tại");

            return res;
        }

        public async Task<SurveyUserInput> CreateUserInput(SurveyUserInputSave val)
        {
            var userInput = _mapper.Map<SurveyUserInput>(val);


            return await CreateAsync(userInput);
        }

        private void SaveLines(SurveyUserInputSave val, SurveyUserInput userinput)
        {
            //remove line
            var lineToRemoves = new List<SurveyUserInputLine>();
            foreach (var existLine in userinput.Lines)
            {
                if (!val.Lines.Any(x => x.Id == existLine.Id))
                    lineToRemoves.Add(existLine);
            }

            foreach (var line in lineToRemoves)
            {
                userinput.Lines.Remove(line);
            }

            foreach (var line in val.Lines)
            {
                if (line.Id == Guid.Empty)
                {
                    var l = _mapper.Map<SurveyUserInputLine>(line);
                    userinput.Lines.Add(l);
                }
                else
                {
                    var l = userinput.Lines.SingleOrDefault(c => c.Id == line.Id);
                    if (l != null)
                    {
                        _mapper.Map(line, l);
                    }
                }
            }

        }

        public async Task UpdateUserInput(Guid id, SurveyUserInputSave val)
        {
            var userInput = await SearchQuery(x => x.Id == id).Include(x => x.Lines).FirstOrDefaultAsync();
            if (userInput == null)
                throw new Exception("Khảo sát không tồn tại");

            userInput = _mapper.Map(val, userInput);

            await UpdateAsync(userInput);
        }

    }
}
