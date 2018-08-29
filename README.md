# URL Parser

This app says whether given site (and sites linked inside it) contains such a word or not.

Enter input parametres and start search for specific word! Multithreading!

Input params:

  - Start URL
  - Max threads count
  - Text to search
  - Max URLs count

Client-server communication implemented on websockets with signalR. 

Used Event Bus for notification about parsing progress, new finded inner links.

Decided not to use authorization and in-memory DB as a storage. So instead were used guid's for each new tab and static dictionaries.

Used Producer Consumer Pattern with Task Completion Source on Actions.

LinksQueueProvider provides link collecting and enqueue them by Breadth-First Search. The innner url structure is similar to a tree We have root link(root nodes) which contains inner links(first-level nodes). Each of them contains inner links too (second-level nodes) and so on. LinkQueueProvider provides BlockingCollection based on SimplePriorityQueue which allows us take items from collection with highest piority (top level nodes first). It's not classic Breadth-First Search algorithm implementation but we have no idle Threads. They take first available  node with highest priority. If we have heavy site(on first level) which process for a while and collect inner links(second level nodes) later than other threads(suppose, they already collect 5-th level links), they will be collect immediately (cause they will have higher priority).

For testing we have to control shared sources(list, dictionaries etc.) to keep them thread safety (if not used collection from System.Collections.Concurrent). Prefer atomic operations to non-atomic. Prevent Threads from idle after finishing their work.
