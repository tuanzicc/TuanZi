using System;

using Microsoft.AspNetCore.Builder;


namespace TuanZi.AspNetCore
{
    public interface IAspUsePack
    {
        void UsePack(IApplicationBuilder app);
    }
}