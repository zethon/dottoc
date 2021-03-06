﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using dotTOC;

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

        [CommandMethod("/exit", "exit the program")]
        [MethodAlias(new string[] { "/q", "/e", "/exit" })]
        public void Exit()
        {
            _controller.Quit();
        }
        [CommandMethod("/format","/format <nick>")]
        [MethodAlias(new string[] {"/format","/f"})]
        public void FormatNickname(string strNickname)
        {
            _controller.TOC.FormatNickname(strNickname);
        }

        [CommandMethod("/help", "this is it")]
        [MethodAlias(new string[] {"/?","/help"})]
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

        [CommandMethod("/login", "/login <username> <password>")]
        [MethodAlias(new string[] {"/l","/login"})]
        public void Login(CommandParser parser)
        {
            if (parser.Parameters.Length < 2)
            {
                Console.WriteLine("Usage: login <username> <password>");
                return;
            }

            _controller.Login(parser.Parameters[0], parser.Parameters[1]);
        }

        [CommandMethod("/notifcations <on|off>","turn on/off buddy status notifications")]
        [MethodAlias(new string[] {"/n","/notifications"})]
        public void Notifications(CommandParser parser)
        {
            if (parser.Parameters.Length == 0)
            {
                Console.WriteLine("Useage: /notifications <on|off>");
                return;
            }

            if (parser.Parameters[0].ToLower() == "on")
            {
                _controller.BuddyNotifications = true;
            }
            else if (parser.Parameters[0].ToLower() == "off")
            {
                _controller.BuddyNotifications = false;
            }
            else
            {
                Console.WriteLine("Useage: /notifications <on|off>");
            }
        }

        [CommandMethod("/reply", "/reply <message>, replies to sender of most recent message")]
        [MethodAlias(new string[] {"/r","/reply"})]
        public void Reply(string strText)
        {
            _controller.Reply(strText);
        }

        [CommandMethod("/send", "/send <username> <message>, send message to user")]
        [MethodAlias(new string[] {"/s","/send"})]
        public void Send(string strText)
        {
            try
            {
                string strName = string.Empty;
                int iSpace = strText.IndexOf(' ');

                if (iSpace != -1)
                {
                    strName = strText.Substring(0, iSpace);
                    strText = strText.Substring(iSpace + 1, strText.Length - (iSpace + 1));

                    _controller.Send(strName, strText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Commands.Send Exception: " + ex.Message);
            }
        }

        [MethodAlias(new string[] { "/status" })]
        public void SetAway(string strAwayText)
        {
            _controller.SetAway(strAwayText);
        }

        [CommandMethod("/current", "/current <username>, sets the user of the current conversation")]
        [MethodAlias(new string[] { "/c", "/current" })]
        public void SetDefault(CommandParser parser)
        {
            if (parser.Parameters.Length > 0)
            {
                string strTemp = parser.Parameters[0];
                if (strTemp.Length < 3)
                {
                    _controller.WriteError("Invalid username.");
                }
                else
                {
                    _controller.CurrentUser = parser.Parameters[0];
                    Console.WriteLine("Default message username: {0}", parser.Parameters[0]);
                }
            }
            else
            {
                if (_controller.CurrentUser == string.Empty)
                {
                    _controller.WriteError("You did not specify a username.");
                }
                else
                {
                    Console.WriteLine(string.Format("Default user: {0}", _controller.CurrentUser));
                }
            }

        }

        [CommandMethod("/buddy", "show online buddie")]
        [MethodAlias(new string[] { "/b", "/buddy" })]
        public void ShowBuddyList()
        {
            lock (this)
            {
                ConsoleColor c = Console.ForegroundColor;

                foreach (Buddy buddy in _controller.BuddyList.Values.OrderBy(x => x.Name))
                {
                    string strMobile = buddy.Class == OscarClass.Wireless ? "(m)" : string.Empty;

                    if (buddy.Online && buddy.IdleTime == 0 && !buddy.MarkedUnavailable)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(string.Format("{0} is online {1}", buddy.Name,strMobile));
                    }
                    else if (buddy.Online && buddy.IdleTime != 0 && !buddy.MarkedUnavailable)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(string.Format("{0} is idle {1}", buddy.Name, strMobile));
                    }
                    else if (buddy.Online && buddy.MarkedUnavailable)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("[{0}] ", DateTime.Now.ToString("HH:mm:ss"));
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(string.Format("{0} is away {1}", buddy.Name, strMobile));
                    }
                }
                Console.ForegroundColor = c;
            }
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
