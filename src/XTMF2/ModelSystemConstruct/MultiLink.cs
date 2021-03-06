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
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Linq;
using XTMF2.Editing;

namespace XTMF2.ModelSystemConstruct
{
    public sealed class MultiLink : Link
    {
        private readonly ObservableCollection<Node> _Destinations;

        public MultiLink(Node origin, NodeHook hook, List<Node> destinations, bool disabled)
            : base(origin, hook, disabled)
        {
            _Destinations = new ObservableCollection<Node>(destinations);
        }

        public ReadOnlyObservableCollection<Node> Destinations =>
            new ReadOnlyObservableCollection<Node>(_Destinations);

        internal bool AddDestination(Node destination, out CommandError? error)
        {
            _Destinations.Add(destination);
            error = null;
            return true;
        }

        internal bool AddDestination(Node destination, int index)
        {
            _Destinations.Insert(index, destination);
            return true;
        }

        internal override void Save(Dictionary<Node, int> moduleDictionary, Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteNumber(OriginProperty, moduleDictionary[Origin!]);
            writer.WriteString(HookProperty, OriginHook!.Name);
            writer.WritePropertyName(DestinationProperty);
            writer.WriteStartArray();
            foreach (var dest in _Destinations)
            {
                writer.WriteNumberValue(moduleDictionary[dest]);
            }
            writer.WriteEndArray();
            if (IsDisabled)
            {
                writer.WriteBoolean(DisabledProperty, true);
            }
            writer.WriteEndObject();
        }

        internal override bool Construct(ref string? error)
        {
            var moduleCount = _Destinations.Count(d => !d.IsDisabled);
            if(OriginHook!.Cardinality == HookCardinality.AtLeastOne)
            {
                if (moduleCount <= 0)
                {
                    error = "At least one module is required as a destination.";
                    return false;
                }
                if(IsDisabled)
                {
                    error = "A required MultiLink is disabled!";
                    return false;
                }
            }
            if(!IsDisabled)
            {
                OriginHook!.CreateArray(Origin!.Module!, moduleCount);
                int index = 0;
                for (int i = 0; i < _Destinations.Count; i++)
                {
                    if (!_Destinations[i].IsDisabled)
                    {
                        OriginHook.Install(Origin!, _Destinations[i], index++);
                    }
                }
            }
            else
            {
                OriginHook!.CreateArray(Origin!.Module!, 0);
            }
            return true;
        }

        internal void RemoveDestination(int i)
        {
            _Destinations.RemoveAt(i);
        }

        internal override bool HasDestination(Node destNode)
        {
            return _Destinations.Contains(destNode);
        }
    }
}
