using System;
using System.Collections.Generic;
using System.Text;

namespace SensingStoreCloud.Dto
{
    public class TreeDto<T>
    {
        public string Text { get; set; }

        public T Id { get; set; }

        public string Type { get; set; }

        public bool isSelected { get; set; }

        public List<TreeDto<T>> Children { get; set; }

    }
}
