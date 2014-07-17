#region copyright
/*
Awful Metro - A Modern UI Something Awful Forums Viewer
Copyright (C) 2014  Tim Miller

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software Foundation,
Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301  USA
*/
#endregion
using System.Threading.Tasks;
using AwfulMetro.Core.Tools;

namespace AwfulMetro.Core.Manager
{
    public interface IAuthenticationManager
    {
        string Status { get; }

        Task<bool> Authenticate(string userName, string password,
            int timeout = Constants.DEFAULT_TIMEOUT_IN_MILLISECONDS);
    }
}