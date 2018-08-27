using System;

using Microsoft.AspNetCore.Builder;

using TuanZi.Core.Packs;


namespace TuanZi.AspNetCore
{
    public abstract class AspTuanPack : TuanPack
    {
        #region Overrides of TuanPack

        public override void UsePack(IServiceProvider provider)
        { }

        #endregion

        public virtual void UsePack(IApplicationBuilder app)
        {
            base.UsePack(app.ApplicationServices);
        }
    }
}