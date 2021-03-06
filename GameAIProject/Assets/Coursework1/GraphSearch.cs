﻿using UnityEngine;
using System.Collections.Generic;

// Code taken from example provided by Jeremy Gow
interface GraphSearch {
	void setGraph(IGraph g); 
	List<int> findPath(int a, int b, bool trace); 
}

public abstract class FrontierSearch : GraphSearch {
	
	protected IGraph graph;
	protected bool trace;
	protected const int NONE = -1;
	
	public void setGraph(IGraph g) {
		graph = g;
	}
	
	public abstract List<int> findPath (int a, int b, bool trace);
	
	protected void log(string s) {
		if (trace)
			Debug.Log (s);
	}
}


public class BFS : FrontierSearch {
	
	protected Queue<int> frontier;
	protected Dictionary<int, int> came_from;
	
	public BFS() {
		
	}
	
	public override List<int> findPath(int start, int goal, bool t) {
		trace = t;
		log ("BFS: looking for path from " + start + " to " + goal);
		
		frontier = new Queue<int>();
		frontier.Enqueue(start);
		came_from = new Dictionary<int, int>();
		came_from[start] = NONE;
		
		bool done = false;
		
		// Main search loop
		while (frontier.Count != 0) {
			int current = frontier.Dequeue ();
			log ("Current node is " + current);
			
			if (current == goal) {
				done = true;
				log ("Found goal node " + goal);
				break;
			}

			List<int> neighbours = graph.neighbours (current);
			log (neighbours.Count + " neighbours found");
			
			foreach (int next in neighbours) {
				if (!came_from.ContainsKey (next)) {
					log ("Adding to " + next + " to frontier");
					frontier.Enqueue (next);
					came_from [next] = current;
					
				} else {
					log ("Already visited " + next); 
				}
			}
		}
		
		// Reconstruct the path
		List<int> path = null;
		if (done) {
			path = new List<int>();
			
			int current = goal;
			
			while (current != start) {
				path.Add(current);
				current = came_from[current];
				if (path.Contains(current)) {
					log ("Error: path contains a loop");
					return null;
				}
			}
			path.Add (start);
		}
		
		path.Reverse();
		
		return path;
	}
}

// Implements PriorityQueue class created by Morten Nobel-Jørgensen
// from: https://github.com/mortennobel/UnityUtils
public class AStarSearch : FrontierSearch
{
	// PriorityQueue<priority, value>
	protected PriorityQueue<float, int> frontier;
	protected Dictionary<int, int> came_from;
	protected Dictionary<int, float> cost_so_far;
	protected float heuristic = 1f;
	
	public AStarSearch()
	{
	}
	
	public override List<int> findPath(int start, int goal, bool t)
	{
		trace = t;
		log ("A* Search: looking for path from " + start + " to " + goal);
		
		frontier = new PriorityQueue<float, int>();
		frontier.Enqueue(start, 0);
		came_from = new Dictionary<int, int>();
		came_from[start] = NONE;
		cost_so_far = new Dictionary<int, float>();
		cost_so_far[start] = 0;
		
		bool done = false;
		
		// Main search loop
		while (!frontier.IsEmpty) 
		{
			int current = frontier.Dequeue ();
			log ("Current node is " + current);
			
			if (current == goal) 
			{
				done = true;
				log ("Found goal node " + goal);
				break;
			}

			List<int> neighbours = graph.neighbours (current);
			log (neighbours.Count + " neighbours found");
			
			foreach (int next in neighbours) 
			{
				float new_cost = cost_so_far[current] + graph.cost (current, next);
				if (!(cost_so_far.ContainsKey(next)) || new_cost < cost_so_far[next]) 
				{
					cost_so_far[next] = new_cost;
					float priority = new_cost + heuristic;
					//log ("Heuristic of " + next + " is: " + heuristic);
					//log ("Adding to " + next + " to frontier, with a cost of " + new_cost);
					frontier.Enqueue (next, priority);
					came_from [next] = current;
					
				} 
				else 
				{
					log ("Already visited " + next); 
				}
			}
		}
		
		// Reconstruct the path
		List<int> path = null;
		if (done) 
		{
			path = new List<int>();
			
			int current = goal;
			
			while (current != start) 
			{
				path.Add(current);
				current = came_from[current];
				if (path.Contains(current)) 
				{
					log ("Error: path contains a loop");
					return null;
				}
			}
			path.Add (start);
		}
		
		path.Reverse();
		
		return path;
	}

	public void setHeuristic(Vector2i x, Vector2i y)
	{
		heuristic = (x - y).magnitude;
	}
}

