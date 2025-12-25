using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEappTV
{
    /// <summary>
    /// Custom parser implementation for the BOOSEappTV interpreter.
    /// </summary>
    /// <remarks>
    /// This parser extends the BOOSE <see cref="Parser"/> to support
    /// application-specific commands, variable reassignment, control-flow
    /// constructs, and method definitions.
    /// It preserves BOOSE’s two-pass execution model by ensuring that
    /// declarations occur at parse/compile time and evaluation is deferred
    /// to runtime.
    /// </remarks>
    public class AppParser : Parser
    {
        /// <summary>
        /// The stored program being constructed during parsing.
        /// </summary>
        private readonly AppStoredProgram program;

        /// <summary>
        /// The command factory used to create command instances.
        /// </summary>
        private readonly CommandFactory factory;

        /// <summary>
        /// Stack used to manage nested method declarations.
        /// </summary>
        private readonly Stack<AppMethod> methodStack = new();

        /// <summary>
        /// Stack used to manage nested conditionals and loop constructs.
        /// </summary>
        private readonly Stack<Command> conditionalStack = new();

        /// <summary>
        /// Initialises a new instance of the <see cref="AppParser"/> class.
        /// </summary>
        /// <param name="factory">
        /// The command factory used to create command instances.
        /// </param>
        /// <param name="program">
        /// The <see cref="AppStoredProgram"/> to populate during parsing.
        /// </param>
        public AppParser(CommandFactory factory, AppStoredProgram program)
            : base(factory, program)
        {
            this.factory = factory;
            this.program = program;
        }

        /// <summary>
        /// Parses a single line of source code and returns the corresponding command.
        /// </summary>
        /// <param name="line">The source line to parse.</param>
        /// <returns>
        /// The parsed <see cref="ICommand"/>, or <c>null</c> for comments or blank lines.
        /// </returns>
        /// <exception cref="ParserException">
        /// Thrown when a syntax or structural error is encountered.
        /// </exception>
        public override ICommand ParseCommand(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            line = line.Trim();
            line = line.TrimStart('\uFEFF');

            // Comment
            if (line.StartsWith("*"))
                return null;

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // IF
            if (parts[0].Equals("if", StringComparison.OrdinalIgnoreCase))
            {
                string condition = line.Substring(2).Trim();

                var ifCmd = new AppIf();
                ifCmd.Set(program, condition);

                ifCmd.IfLine = program.Count;
                conditionalStack.Push(ifCmd);
                program.Add(ifCmd);

                return ifCmd;
            }

            // ELSE
            if (parts[0].Equals("else", StringComparison.OrdinalIgnoreCase))
            {
                if (conditionalStack.Count == 0 || conditionalStack.Peek() is not AppIf ifCmd)
                    throw new ParserException("else without matching if");

                var elseCmd = new AppElse();
                elseCmd.Set(program, null);

                ifCmd.ElseLine = program.Count;
                elseCmd.MatchingIf = ifCmd;

                program.Add(elseCmd);
                return elseCmd;
            }

            // END IF
            if (line.Equals("end if", StringComparison.OrdinalIgnoreCase))
            {
                if (conditionalStack.Count == 0 || conditionalStack.Peek() is not AppIf ifCmd)
                    throw new ParserException("end if without matching if");

                conditionalStack.Pop();

                var endIf = new AppEndIf();
                endIf.Set(program, null);

                int endLine = program.Count;
                ifCmd.EndIfLine = endLine;

                if (ifCmd.ElseLine >= 0)
                    ((AppElse)program[ifCmd.ElseLine]).EndIfLine = endLine;

                program.Add(endIf);
                return endIf;
            }

            // WHILE
            if (parts[0].Equals("while", StringComparison.OrdinalIgnoreCase))
            {
                string condition = line.Substring(5).Trim();

                var whileCmd = new AppWhile();
                whileCmd.Set(program, condition);

                program.Add(whileCmd);
                whileCmd.WhileLine = program.Count - 1;

                conditionalStack.Push(whileCmd);
                return whileCmd;
            }

            // END WHILE
            if (line.Equals("end while", StringComparison.OrdinalIgnoreCase))
            {
                var whileCmd = (AppWhile)conditionalStack.Pop();

                var endWhile = new AppEndWhile();
                endWhile.Set(program, null);

                program.Add(endWhile);

                whileCmd.EndWhileLine = program.Count - 1;
                endWhile.MatchingWhile = whileCmd;

                return endWhile;
            }

            // FOR
            if (parts[0].Equals("for", StringComparison.OrdinalIgnoreCase))
            {
                string forParams = line.Substring(3).Trim();

                var forCmd = new AppFor();
                forCmd.Set(program, forParams);

                program.Add(forCmd);
                forCmd.ForLine = program.Count - 1;

                conditionalStack.Push(forCmd);
                return forCmd;
            }

            // END FOR
            if (line.Equals("end for", StringComparison.OrdinalIgnoreCase))
            {
                if (conditionalStack.Count == 0 || conditionalStack.Peek() is not AppFor forCmd)
                    throw new ParserException("end for without matching for");

                conditionalStack.Pop();

                var endFor = new AppEndFor();
                endFor.Set(program, null);

                program.Add(endFor);

                forCmd.EndForLine = program.Count - 1;
                endFor.MatchingFor = forCmd;

                return endFor;
            }

            // METHOD
            if (parts[0].Equals("method", StringComparison.OrdinalIgnoreCase))
            {
                // Example:
                // method int testMethod int one, int two
                string header = line.Substring(6).Trim();
                var tokens = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length < 2)
                    throw new ParserException("Invalid method header.");

                string returnType = tokens[0].Trim().ToLower();
                string methodName = tokens[1].Trim().Trim(',');

                var m = new AppMethod();
                m.Set(program, header);

                var def = new AppStoredProgram.MethodDef
                {
                    Name = methodName,
                    ReturnType = returnType,
                    MethodLine = program.Count
                };

                int i = 2;
                while (i < tokens.Length)
                {
                    string pType = tokens[i].Trim().ToLower();
                    i++;

                    if (i >= tokens.Length)
                        throw new ParserException("Invalid method parameter list.");

                    string pName = tokens[i].Trim().Trim(',');
                    i++;

                    def.Params.Add((pType, pName));
                }

                m.Def = def;
                program.RegisterMethod(def);

                program.EnsureVariableExistsForType(def.ReturnType, def.Name);
                foreach (var p in def.Params)
                    program.EnsureVariableExistsForType(p.Type, p.Name);

                program.Add(m);
                methodStack.Push(m);
                return m;
            }

            // END METHOD
            if (line.Equals("end method", StringComparison.OrdinalIgnoreCase))
            {
                if (methodStack.Count == 0)
                    throw new ParserException("end method without matching method");

                var m = methodStack.Pop();

                var endM = new AppEndMethod();
                endM.Set(program, null);

                program.Add(endM);

                m.Def.EndMethodLine = program.Count - 1;
                program.RegisterMethod(m.Def);

                return endM;
            }

            // CALL
            if (parts[0].Equals("call", StringComparison.OrdinalIgnoreCase))
            {
                string callParams = line.Substring(4).Trim();

                var call = new AppCall();
                call.Set(program, callParams);

                program.Add(call);
                return call;
            }

            // REASSIGNMENT
            if (parts.Length >= 3 && parts[1] == "=" && program.VariableExists(parts[0]))
            {
                string varName = parts[0];
                string rhsRaw = string.Join(" ", parts, 2, parts.Length - 2);

                string rhs = program.TidyExpression(rhsRaw);

                var assign = new AppAssign(varName, rhs);
                assign.Set(program, null);
                program.Add(assign);

                AppConsole.WriteLine(
                    $"[DEBUG] Assignment command queued: {varName} = {rhs}"
                );

                return assign;
            }

            // NORMAL COMMAND
            ICommand command = factory.MakeCommand(parts[0]);

            if (command == null)
                throw new ParserException(
                    $"unknown command '{parts[0]}' at line {program.PC + 1}"
                );

            string parameters = line.Substring(parts[0].Length).Trim();
            command.Set(program, parameters);
            command.Compile();

            return command;
        }
    }
}
