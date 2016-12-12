using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Quartz
{
    interface CDTJob : IJob
    {
        void Start(string cronExpression);
    }
}
