using System;
using System.IO;

public interface IRegistro<T> where T : IComparable<T>, new()
{
    bool LerRegistro(StreamReader arq);
    void GravarRegistro(StreamWriter arq);
}