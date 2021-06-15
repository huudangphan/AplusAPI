using System;

namespace Apzon.Libraries.HDBConnection.Interfaces
{
    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}