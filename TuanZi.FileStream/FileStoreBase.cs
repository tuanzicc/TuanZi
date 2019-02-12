using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TuanZi.AspNetCore.Http;
using TuanZi.Data;
using TuanZi.Entity;
using TuanZi.Extensions;

namespace TuanZi.Entity
{
    public abstract class FileStoreBase<TFile>
        where TFile : FileBase
    {
        private readonly IRepository<TFile, Guid> _FileRepository;
        private readonly IServiceProvider _ServiceProvider;

        protected FileStoreBase(IRepository<TFile, Guid> fileRepository, IServiceProvider serviceProvider)
        { 
            _FileRepository = fileRepository;
            _ServiceProvider = serviceProvider;
        }

        public async Task<Guid?> CreateAsync(string name)
        {
            Guid? id = null;
            var context = _ServiceProvider.GetHttpContext();
            if (name.HasValue())
            {
                var formFile = context.Request.Form.Files[name];
                id = await CreateAsync(formFile, commit);
            }
            return id;
        }

        public async Task<Guid?> CreateAsync(IFormFile formFile, TFile file = null)
        {
            Guid? id = null;
            var context = _ServiceProvider.GetHttpContext();
            if (formFile != null && formFile.Length > 0 && context != null)
            {
                if (file == null)
                {
                    file = (TFile) new FileBase();
                }
                if (context.User.Identity.IsAuthenticated)
                {
                    file.AppId = context.User.GetAppId<Guid>();
                    file.UserId = context.User.GetUserId<Guid>();
                }
                file.Name = formFile.FileName;
                file.ContentType = formFile.ContentType;
                file.ContentLength = formFile.Length;
                file.Extension = Path.GetExtension(formFile.FileName);

                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    file.Binary = ms.ToArray();
                }
                var result = await _FileRepository.InsertAsync(file);
                if (result > 0)
                {
                    id = file.Id;
                }
            }

            return id;
        }

        
    }
}