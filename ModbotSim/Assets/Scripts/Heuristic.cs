using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeuristicD {
	
	private Dictionary<Node, float> heuristicCost;

	public HeuristicD(GenerateGraph graph) {
		heuristicCost = new Dictionary<Node, float>();

		PriorityQueue<Node> pq = new PriorityQueue<Node>(graph.Size ());
		Dictionary<Node, float> cost_so_far = new Dictionary<Node, float> ();
		pq.queue(0.0f, graph.endNode);
		cost_so_far.Add (graph.endNode, 0.0f);
		while (pq.getSize() > 0) {
			Node current = pq.dequeue();
			heuristicCost[current] = cost_so_far[current];
			for (int i = 0; i < current.neighbors.Count; i++) {
				float new_cost = cost_so_far[current] + Node.distanceBetweenNodes(current, current.neighbors[i]);
				if (!cost_so_far.ContainsKey(current.neighbors[i]) || new_cost < cost_so_far[current.neighbors[i]]) {
					cost_so_far[current.neighbors[i]] = new_cost;
					pq.queue(new_cost, current.neighbors[i]);
				}	
			}
		}
	}

	public float Estimate(Node n) {
		int numCarsClaiming = PathPlanningDataStructures.nodeToCount [n.position];
		if (numCarsClaiming > 0) {
			Debug.Log (numCarsClaiming + " claiming " + n.position);
			Debug.Log ("Original Heuristic Cost: " + heuristicCost [n]);
			return 1.5f * ((PathPlanningDataStructures.nodeToCount [n.position] * 0.1f) + 1.0f) * heuristicCost [n] /
				ItemsAI.getReduction (n);
		} else {
			return heuristicCost [n] / ItemsAI.getReduction (n);
		}
	}
}