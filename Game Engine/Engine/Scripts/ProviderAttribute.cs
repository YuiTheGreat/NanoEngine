﻿using Game_Engine.Engine.Services.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine.Engine.Scripts
{
    class ProviderAttribute : ScriptAttribute
    {

        public readonly Type[] InjectTypes;
        public ProviderAttribute(params Type[] types)
        {
            InjectTypes = types;
        }
    }
}
