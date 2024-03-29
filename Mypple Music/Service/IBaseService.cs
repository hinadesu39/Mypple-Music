﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mypple_Music.Service
{
    public interface IBaseService<TEntity>
    {
        Task<Uri> UploadAsync(string url);

        Task<FileExistsResponse> FileExistsAsync(long fileSize, string sha256Hash);

        Task<TEntity?> GetByIdAsync(Guid id);

        Task<List<TEntity>> GetAllAsync();

        Task<List<TEntity>> GetByNameAsync(string name);

        Task<bool> DeleteAsync(Guid id);
    }
}
