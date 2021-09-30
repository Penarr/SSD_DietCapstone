using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDietCapstone
{
    public class Food
    {

        public Food(int servingSize, int calsperServing, int proteinPerServing, int carbsPerServing, int fatPerServing)
        {
            this.servingSize = servingSize;
            this.calsPerServing = calsPerServing;
            this.proteinPerServing = proteinPerServing;
            this.carbsPerServing = carbsPerServing;
            this.fatPerServing = fatPerServing;
        }

        public int servingSize { get; set; }

        public int calsPerServing { get; set; }
        public int proteinPerServing { get; set; }
        public int carbsPerServing { get; set; }
        public int fatPerServing { get; set; }
    }
}
