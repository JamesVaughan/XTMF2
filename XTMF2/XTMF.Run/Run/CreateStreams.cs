﻿/*
    Copyright 2017 University of Toronto

    This file is part of XTMF2.

    XTMF2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    XTMF2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with XTMF2.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;

namespace XTMF2.Run.Run
{
    public static class CreateStreams
    {
        /// <summary>
        /// Create a new named pipe host
        /// </summary>
        /// <param name="name">The name for the pipe</param>
        /// <param name="stream">The resulting stream</param>
        /// <param name="error">An error message if there is an exception</param>
        /// <returns>True if successful, false with message otherwise</returns>
        public static bool CreateNewNamedPipeHost(string name, out Stream stream, ref string error)
        {
            try
            {
                NamedPipeServerStream host = new NamedPipeServerStream(name, PipeDirection.InOut);
                stream = host;
                return true;
            }
            catch (IOException e)
            {
                error = e.Message;
            }
            stream = null;
            return false;
        }

        /// <summary>
        /// Create a client for a named pipe.
        /// </summary>
        /// <param name="name">The name of the pipe to connect to.</param>
        /// <param name="stream">The resulting stream</param>
        /// <param name="error">An error message if there is a problem</param>
        /// <returns>True if successful, false with message otherwise</returns>
        public static bool CreateNamedPipeClient(string name, out Stream stream, ref string error)
        {
            try
            {
                // Connect to the named pipe server
                var client = new NamedPipeClientStream(".", name, PipeDirection.InOut);
                stream = client;
                return true;
            }
            catch (IOException e)
            {
                error = e.Message;
            }
            stream = null;
            return false;
        }
    }
}
