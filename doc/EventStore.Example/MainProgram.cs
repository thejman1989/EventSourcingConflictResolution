namespace EventStore.Example
{
	using System;
	using System.Transactions;
	using Dispatcher;
    using System.Collections.Generic;
    using System.Diagnostics;

	internal static class MainProgram
	{
		private static readonly Guid StreamId = Guid.Parse("9030a52e-7da4-4a15-bf7f-6ed28764c6bf");// Guid.NewGuid(); // aggregate identifier 
		private static readonly byte[] EncryptionKey = new byte[]
		{
			0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xf
		};
		private static IStoreEvents store;

		private static void Main()
		{
			using (var scope = new TransactionScope())
			using (store = WireupEventStore())
			{
                //OpenOrCreateStream();
                //AppendToStream();
                //AppendToStream();
                //AppendToStream();
                //AppendToStream();
                //AppendToStream();
                //AppendToStream();
                //AppendToStream();
				//TakeSnapshot();
				//LoadFromSnapshotForwardAndAppend();
               // GetEventFromStore();
				scope.Complete();
			}
		}

		private static IStoreEvents WireupEventStore()
		{
			 return Wireup.Init()
				.LogToOutputWindow()
                .UsingSqlPersistence("MYSQL")
					.EnlistInAmbientTransaction() // two-phase commit
					.InitializeStorageEngine()
					.UsingJsonSerialization()
						.Compress()
						//.EncryptWith(EncryptionKey)
				.HookIntoPipelineUsing(new[] { new AuthorizationPipelineHook() })
				.UsingAsynchronousDispatchScheduler()
					.DispatchTo(new DelegateMessageDispatcher(DispatchCommit))
				.Build();
		}
		private static void DispatchCommit(Commit commit)
		{
			// This is where we'd hook into our messaging infrastructure, such as NServiceBus,
			// MassTransit, WCF, or some other communications infrastructure.
			// This can be a class as well--just implement IDispatchCommits.
			try
			{
				foreach (var @event in commit.Events)
					Console.WriteLine(Resources.MessagesDispatched + ((SomeDomainEvent)@event.Body).Value);
			}
			catch (Exception)
			{
				Debug.WriteLine(Resources.UnableToDispatch);
			}
		}

		private static void OpenOrCreateStream()
		{
			// we can call CreateStream(StreamId) if we know there isn't going to be any data.
			// or we can call OpenStream(StreamId, 0, int.MaxValue) to read all commits,
			// if no commits exist then it creates a new stream for us.
			using (var stream = store.OpenStream(StreamId, 0, int.MaxValue))
			{
				var @event = new SomeDomainEvent { Value = "Initial event." };

				stream.Add(new EventMessage { Body = @event });
				stream.CommitChanges(Guid.NewGuid());
			}
		}
		private static void AppendToStream()
		{
			using (var stream = store.OpenStream(StreamId, int.MinValue, int.MaxValue))
			{
				var @event = new SomeDomainEvent { Value = "Second event." };

				stream.Add(new EventMessage { Body = @event });
				stream.CommitChanges(Guid.NewGuid());
			}
		}
		private static void TakeSnapshot()
		{
			var memento = new AggregateMemento { Value = "snapshot" };
			store.Advanced.AddSnapshot(new Snapshot(StreamId, 2, memento));
		}
		private static void LoadFromSnapshotForwardAndAppend()
		{
			var latestSnapshot = store.Advanced.GetSnapshot(StreamId, int.MaxValue);

			using (var stream = store.OpenStream(latestSnapshot, int.MaxValue))
			{
				var @event = new SomeDomainEvent { Value = "Third event (first one after a snapshot)." };
               
				stream.Add(new EventMessage { Body = @event });
				stream.CommitChanges(Guid.NewGuid());
			}
		}

        private static void GetEventFromStore()
        {
            Console.WriteLine("Store String: " + store.GetType().ToString());

            using (var stream = store.OpenStream(StreamId, 0 , int.MaxValue))
            {
                Console.WriteLine("Stream String: " + stream.ToString());
                OptimisticEventStream stream2 = (OptimisticEventStream)stream;
                foreach (var anEvent in stream.CommittedEvents)
                {
                    Console.WriteLine("Event = " + anEvent.Body);
                }
            }

            //OptimisticEventStore oStore = (OptimisticEventStore)store;

            //IEnumerable<Commit> commits = oStore.GetFrom(StreamId, 0, int.MaxValue);
            //IEnumerable<Commit> commits2 = oStore.Advanced.GetFrom(DateTime.MinValue);
            //Console.WriteLine("Stream id: " + StreamId);

            //foreach (Commit comm in commits2)
            //{
            //    Console.WriteLine("StreamId:        " + comm.StreamId);
            //    Console.WriteLine("StreamRevision:  " + comm.StreamRevision);
            //    Console.WriteLine("Commit:          " + comm.CommitId);
            //    Console.WriteLine("Stamp:           " + comm.CommitStamp);
            //    Console.WriteLine("Body:            " + comm.Events[0]);
            //    Console.WriteLine("Headers:         " + comm.Headers.ToString());

            //    //How to deserialize? Without casting. Generic function getValue(). 
            //    SomeDomainEvent domainEvent = (SomeDomainEvent)(comm.Events[0].Body);

            //    var test = ConstructEvent<SomeDomainEvent>();
            //    Console.WriteLine("SDE:             " + domainEvent.Value);
            //    Console.WriteLine("----------------------------------------------------");
            //}
        }

        private static TEvent ConstructEvent<TEvent>()
        {
            return (TEvent)Activator.CreateInstance(typeof(TEvent), true);
        }
	}
}