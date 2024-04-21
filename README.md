# Winforms Graph Visualiser

A way to visualise graphs onto a user control in winforms (using a GraphView control).

(An example project is included in this repo).

## Example Usage

Adding nodes to the graph:
```cs
private void AddNodesExample()
{
    GraphNode node1 = new GraphNode("a", (0, 0), 30, 30);
    GraphNode node2 = new GraphNode("b", (100, 0), 30, 30);
    graphView1.AddNode(node1);
    graphView1.AddNode(node2);
}
```

![image](https://github.com/mohfez/winform-graph-visualiser/assets/150836596/22045f1f-5f46-4983-8948-3d4d7ee7bce1)


Adding an edge to the graph:
```cs
private void AddEdgeExample()
{
    GraphNode node1 = new GraphNode("a", (0, 0), 30, 30);
    GraphNode node2 = new GraphNode("b", (100, 0), 30, 30);
    graphView1.AddEdge(node1, node2);
}
```

![image](https://github.com/mohfez/winform-graph-visualiser/assets/150836596/30ac7af1-d329-4f7e-a2e9-652b4fb68671)


## Custom Graph Class

Creating custom graph helper classes is the best way to use this library.

For example, the `TreeGraph` folder is not required by the library but can be used to position nodes to form binary trees easier:

![image](https://github.com/mohfez/winform-graph-visualiser/assets/150836596/892d757c-f5f1-4843-a637-36a57cd660cb)


## More Info

More info about the `GraphNode` and `GraphView` classes.

GraphNode has 2 main constructors:
```cs
public GraphNode(string text, int minWidth = 34, int minHeight = 34) {}
public GraphNode(string text, (int x, int y) position, int minWidth, int minHeight) {}
```

GraphView has 2 optimisation options:
```cs
public bool renderOffScreen = false;
public bool autoInvalidate = true;
```
`renderOffScreen` - (default: false) If the nodes are off-screen then don't render them along with their arrows.

`autoInvalidate` - (default: true) If doing a large operation such as adding many nodes at once then it's best to turn this off before then back on after.

`autoInvalidate is used for automatically painting the nodes and arrows onto the screen but is not recommended to be on during large operations (e.g. adding/removing many edges/nodes, etc.).`

`Although it is not very noticeable, so you can leave it on if you want.`

Adding the user control:

![image](https://github.com/mohfez/winform-graph-visualiser/assets/150836596/a4b2f348-6364-4318-964c-2e5def671a33)
