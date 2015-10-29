// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Threading.Tasks;
using System.Threading;
#if SPREADS
using Spreads;
#endif
namespace Spreads {

#if SPREADS

    //public interface IAsyncEnumerator<T> : Spreads.IAsyncEnumerator<T> {
    //}

#else
    /// <summary>
    /// Asynchronous version of the IEnumerator&lt;T&gt; interface, allowing elements to be
    /// retrieved asynchronously.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public interface IAsyncEnumerator<
#if !NO_VARIANCE
out
#endif
        T> : IEnumerator<T>, IDisposable
    {
        /// <summary>
        /// Advances the enumerator to the next element in the sequence, returning the result asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel the operation.</param>
        /// <returns>
        /// Task containing the result of the operation: true if the enumerator was successfully advanced 
        /// to the next element; false if the enumerator has passed the end of the sequence.
        /// </returns>
        Task<bool> MoveNext(CancellationToken cancellationToken);

    }
#endif
    
    internal static class AsyncEnumeratorMoveExtensions {
        /// <summary>
        /// Helper method to move an async cursor using sync method, but to return a task while keeping Spreads contracts
        /// </summary>
        public static Task<bool> MoveNext<T>(this IAsyncEnumerator<T> e, CancellationToken ct, bool isAsync) {
            if (isAsync) {
                return e.MoveNext(ct);
            } else {
                try {
                    if (e.MoveNext()) {
                        return Task.FromResult(true);
                    } else {
                        return Task.FromResult(false);
                    }
                } catch (Exception ex) {
                    return TaskExt.Throw<bool>(ex);
                }
            }
        }
    }
}
