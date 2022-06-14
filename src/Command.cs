using System;
using System.Collections.Generic;
using System.Text;

namespace File_Manager
{
    public abstract class Command
    {
        public abstract string Name { get; }

        public virtual bool CanHandle(string cmd)
        {
            return false;
        }
        public abstract void Handle(string parsedArgument);
    }
}
