﻿using ArtHouse2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtHouse2.Data
{
    internal interface IArtTypeRepository
    {
        Task<List<ArtType>> GetArtTypes();
        Task<ArtType> GetArtType(int ID);
        Task AddArtType(ArtType ArtTypeToAdd);
        Task UpdateArtType(ArtType ArtTypeToUpdate);
        Task DeleteArtType(ArtType ArtTypeToDelete);
    }
}
