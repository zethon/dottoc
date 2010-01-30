﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConsoleAim
{
    class Commands
    {
        private class CommandMethodAttribute : System.Attribute
        {
            private string _name = string.Empty;
            public string Name
            {
                get { return _name; }
            }

            private string _summary = string.Empty;
            public string Summary
            {
                get { return _summary; }
            }

            private string _details = string.Empty;
            public string Details
            {
                get { return _details; }
            }

            public CommandMethodAttribute(string strName, string strSummary, string strDetails)
            {
                _name = strName;
                _summary = strSummary;
                _details = strDetails;
            }

            public CommandMethodAttribute(string strName, string strSummary)
            {
                _name = strName;
                _summary = strSummary;
            }
        }

        private class MethodAliasAttribute : System.Attribute
        {
            public string[] Aliases;

            public MethodAliasAttribute(string[] aliases)
            {
                Aliases = aliases;
            }
        }

        Controller _controller = null;

        public Commands(Controller controller)
        {
            _controller = controller;
        }

        [CommandMethod("help", "this is it")]
        [MethodAlias(new string[] {"?","ayudar","/?"})]
        public void Help()
        {
            Type t = this.GetType();

            foreach (MethodInfo info in t.GetMethods())
            {
                object[] obs = info.GetCustomAttributes(false);

                foreach (object o in obs)
                {
                    CommandMethodAttribute rcma = o as CommandMethodAttribute;

                    if (rcma != null)
                    {
                        Console.WriteLine("{0} - {1}", rcma.Name, rcma.Summary);
                    }
                }

            }
        }

        [CommandMethod("login", "login <username> <password>")]
        [MethodAlias(new string[] { "li", "/l" })]
        public void Login(CommandParser parser)
        {
            if (parser.Parameters.Length < 2)
            {
                Console.WriteLine("Usage: login <username> <password>");
                return;
            }

            _controller.Login(parser.Parameters[0], parser.Parameters[1]);
        }

        [CommandMethod("reply", "reply <message>, replies to sender of most recent message")]
        [MethodAlias(new string[] { "r", "/r" })]
        public void Reply(string strText)
        {
            _controller.Reply(strText);
        }

        [CommandMethod("send", "send <username> <message>, send message to user")]
        [MethodAlias(new string[] { "s", "/s" })]
        public void Send(string strText)
        {
            try
            {
                int iSpace = strText.IndexOf(' ');

                if (iSpace != -1)
                {

                }
                string[] strTemp = Regex.Split(strText, @"\s");
                //if (parser.Parameters.Length > 1)
                //{
                //    ArrayList temp = new ArrayList(parser.Parameters.ToArray());
                //    temp.RemoveAt(0);
                //    string strMessage = string.Join(" ", (string[])temp.ToArray());
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commands.Send Exception: " + ex.Message);
            }
        }

        [CommandMethod("exit", "exit the program")]
        [MethodAlias(new string[] { "q", "quit", "/q" })]
        public void Exit()
        {
            _controller.Quit();
        }

        public void ExecuteCommand(string strCommand)
        {
            CommandParser parser = new CommandParser(strCommand, this);
            parser.Parse();

            MethodInfo mi = this.GetType().GetMethod(parser.ApplicationName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

            if (mi == null)
            {
                foreach (MethodInfo mit in this.GetType().GetMethods())
                {
                    foreach (object obj in mit.GetCustomAttributes(false))
                    {
                        MethodAliasAttribute maa = obj as MethodAliasAttribute;

                        if (maa != null)
                        {
                            if (maa.Aliases != null && maa.Aliases.Contains(parser.ApplicationName))
                            {
                                mi = mit;
                                break;
                            }
                        }
                    }

                    if (mi != null)
                    {
                        break;
                    }
                }
            }

            if (mi != null)
            {
                try
                {
                    if (mi.GetParameters().Length == 0)
                    {
                        mi.Invoke(this, null);
                    }
                    else
                    {
                        object[] oparams = new object[mi.GetParameters().Length];

                        foreach (ParameterInfo m in mi.GetParameters())
                        {
                            if (m.ParameterType == typeof(string))
                            {
                                //oparams[oparams.Length - 1] = parser.Parameters[oparams.Length - 1];
                                oparams[oparams.Length - 1] = parser.WorkingString;
                            }
                            else if (m.ParameterType == typeof(CommandParser))
                                oparams[oparams.Length - 1] = parser;
                        }

                        if (oparams != null)
                            mi.Invoke(this, oparams);

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Commands.ExecuteCommand exception: " + e.Message);
                }
            }
            else
            {
                Console.WriteLine(@"Unknown command");
            }

        }
    }
}