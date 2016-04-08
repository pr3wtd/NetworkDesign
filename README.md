The aim of this project is to make up an algorithm to find the optimal choice of links between nodes in a given
network in such a way that the overall cost of demands (from one node to another) is the lowest possible.
In order to achieve that goal I have decided to implement modified label correcting algorithm to calculate
the cheapest price for each demand and then using simulated annealing algorithm look for the best solution.


##### PROGRAM USAGE #####

1) Set the path of the input file 

STRUCTURE OF INPUT FILE : 

NODES = {nr_of_nodes}

EDGES = {nr_of_edges}
ID START_NODE END_NODE COST_OF_ONE_MODULE

ORDERS = {nr_of_orders}
ID SOURCE_NODE DESTINATION_NODE SIZE

2) Define the value of temperature and alpha (simply speaking: greater values mean more simulated annealing iterations so higher probability of receiving better solution ) 

Program will start processing the input network and calcaluting solution. Output will be written to results.txt file.

There is a input network :
- links ( an edge between two nodes ) that has defined capacity (how much size of the demand can contain the praticular edge) and
cost of one module
- orders - some resource demand between two nodes.

In the algorithm the demand is described in the order fields: source node, destination node and size. 
When calculatin the solution, the order size is taken into account: such number of the link mudules has to be bought that their capacity
is at least the size of the demand. There is also free space left in a link that is used to fill another order.
Therefore there is a possibility that, a demand can "go through" a link for free - no link module is bought.

Let's have a look at this example: 

We have three nodes and links between each other in two directions. There are two orders described below.

	1 ----- 2
	|	    |
	 - -3- -	

<b>Links:</b>
<br>
	1) 1-2 has capacity 5 and cost 1<br>
	2) 2-3 has capacity 2 and cost 2<br>
	3) 1-3 has capacity 10 and cost 3<br>

<b>Orders:</b>
<br>
	1) Demand of size 18 from node 1 to 3<br>
	2) Demand of size 1 from node 3 to 2<br>

	
Fill the 1st order and then the 2nd order : 
If we buy 2 modules of link 1-3 (overall cost 6) there are 2 size units left. Therefore
the second demand of size 1 from node 3 to 2 can go through link 1-3 (for free) and link 1-2
(one module, cost 1).
Overall cost for all demands is: 6 + 1 = 7.

But if we consider the 2nd order first:
We buy one module of link 2-3 (cost 2) because it's the cheapest solution for this order and
then, considering order 1, we buy 2 modules of link 1-3 (cost 6).
Overall cost for all demands is: 2 + 6 = 8.

So we clearly see that different sequences of demands give different solutions.
That's why there is simulated annealing algorithm that provides the randomness
of orders. It also forces the first order in the sequence to go through a random link of the source node.


