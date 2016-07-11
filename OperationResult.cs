//  The MIT License (MIT)
//  
//  Copyright (c) 2016 CaptiveAire Limitied
//  
//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//  the Software, and to permit persons to whom the Software is furnished to do so,
//  subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//  FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//  COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//  IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//  

using System.Collections.Generic;
using System.Linq;

namespace SuperClean
{
    public interface IOperationResult
    {
        IReadOnlyCollection<string> Messages { get; }
    }

    public interface IOperationResultSuccess : IOperationResult
    {
    }

    public interface IOperationResultFailure : IOperationResult
    {
    }

    public class OperationResult : IOperationResult
    {
        protected OperationResult(IReadOnlyCollection<string> messages = null)
        {
            this.Messages = messages ?? new List<string>(0);
        }

        public IReadOnlyCollection<string> Messages { get; }

        public static IOperationResultFailure Failure(IEnumerable<string> messages = null)
        {
            return new OperationResultFailure(messages?.ToList());
        }

        public static IOperationResultSuccess Success(IEnumerable<string> messages = null)
        {
            return new OperationResultSuccess(messages?.ToList());
        }

        class OperationResultSuccess : OperationResult, IOperationResultSuccess
        {
            public OperationResultSuccess(IReadOnlyCollection<string> messages)
                : base(messages)
            {
            }
        }

        class OperationResultFailure : OperationResult, IOperationResultFailure
        {
            public OperationResultFailure(IReadOnlyCollection<string> messages)
                : base(messages)
            {
            }
        }
    }
}