using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mqtt.Test
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PersonDto
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }
    public class AutomapperTest
    {
        public void MapperStart()
        {
            //var mapper = new MapperConfiguration()

            var config = new MapperConfiguration(cfg => 
            {
                //cfg.CreateMissingTypeMaps
                cfg.CreateMap<Person, PersonDto>();
            });

            var mapper = config.CreateMapper();

            var william = new Person { Name = "william", Age = 37 };
            var dto = mapper.Map<PersonDto>(william);

        }
    }
}
