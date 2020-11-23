using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Utilities
{
    public interface IViewRenderService
    {
        string Render<T>(string name, T model) ;
    }
}
