using CrudService.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CrudService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrudController : Controller
    {
        private readonly ILogger<CrudController> _logger;

        public CrudController(ILogger<CrudController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetPerson")]
        public IEnumerable<IPerson> Get()
        {
            return ReadJson();
        }


        [HttpPost(Name = "PostPerson")]
        public async Task<bool> Post(IList<Person> persons)
        {
            int newId = 1;
            List<Person> updateSource = (List<Person>?)ReadJson() ?? new List<Person>();
            var comparer = new PersonComparer();
            var unique = persons.Except(updateSource, comparer).ToList();
            if(unique.Count == 0)
            {
                _logger.LogWarning("Duplicate record");
                return false;
            }

            //find and update new Id
            if(updateSource.Count > 0)
            {
                updateSource = updateSource.OrderBy(x => x.Id).ToList();
                newId = updateSource[updateSource.Count - 1].Id + 1;
            }
            foreach(var item in unique)
            {
                item.Id = newId;
                newId++;
            }            
            updateSource.AddRange(unique);

            await WriteJson(updateSource);
            
            return true;
        }


        [HttpPut(Name = "UpdatePerson")]
        public async Task<bool> Put(IList<Person> persons)
        {
            List<Person> updateSource = (List<Person>?)ReadJson() ?? new List<Person>();
            var idMatch = persons.Where(x => updateSource.Select(y => y.Id).Contains(x.Id));
            if (idMatch.Count() > 0)
            {
                foreach (var person in idMatch)
                {
                    updateSource.RemoveAll(x => x.Id == person.Id);
                    updateSource.Add(person);
                }

                await WriteJson(updateSource);
                return true;
            }
            return false;
        }


        [HttpDelete(Name = "DeletePerson")]
        public async Task<bool> Delete(int id)
        {
            List<Person> updateSource = (List<Person>?)ReadJson() ?? new List<Person>();
            var idMatch = updateSource.Where(x => x.Id == id);
            if (idMatch.Count() > 0)
            {
                updateSource.Remove(idMatch.First());
                await WriteJson(updateSource);
                return true;
            }
            return false;
        }

        //Move below method to repository class

        private async Task WriteJson(List<Person> updateSource)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(updateSource, new JsonSerializerOptions() { WriteIndented = true });
                using (StreamWriter writer = new StreamWriter("data/person.json"))
                {
                    await writer.WriteLineAsync(jsonString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updatin record " + ex.Message);
                throw;
            }
        }

        private IEnumerable<IPerson> ReadJson()
        {
            IList<Person>? source = new List<Person>();

            using (StreamReader r = new StreamReader("data/person.json"))
            {
                string persons = r.ReadToEnd();
                source = JsonSerializer.Deserialize<List<Person>>(persons);
            }

            return source ?? new List<Person>();
        }

    }
}
