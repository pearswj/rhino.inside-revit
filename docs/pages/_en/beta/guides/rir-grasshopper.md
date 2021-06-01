---
title: Grasshopper in Revit
subtitle: How to use Grasshopper inside Revit
order: 11
group: Essentials
thumbnail: /static/images/guides/rir-grasshopper.png
ghdef: rir-grasshopper.ghx
---

{% include youtube_player.html id="VsE5uWQ-_oM" %}

## Revit-Aware Components

The Revit-aware component icons help identifying the action that the component performs. As shown below, the base color shows the type of action (Query, Analyze, Modify, Create). There are a series of badges applied to icons as well, that show Type, Identity, or other aspects of the data that the component is designed to work with:

![]({{ "/static/images/guides/rir-grasshopper-conventions@2x.png" | prepend: site.baseurl }}){: class="small-image"}

For example, this is how the Parameter, Query, Analyze, Modify, and Create components for {% include ltr/comp.html uuid='15ad6bf9' %} are shown:

![]({{ "/static/images/guides/rir-grasshopper-compcolors@2x.png" | prepend: site.baseurl }}){: class="small-image"}

### Pass-through Components

In some cases, a special type of pass-through component has been used that combines the Analyze, Modify, Create actions in to one component. This helps reducing the number of components and avoid cluttering the interface. These components have a split background like {% include ltr/comp.html uuid='4cadc9aa' %} or {% include ltr/comp.html uuid='222b42df' %}:

![]({{ "/static/images/guides/rir-grasshopper-passthrucomps@2x.png" | prepend: site.baseurl }}){: class="small-image"}

Let's take a look at {% include ltr/comp.html uuid='222b42df' %} as an example. These components accept two groups of inputs. The first input parameter is the Revit element that this component deals with, in this case {% include ltr/comp.html uuid='b18ef2cc' %}. Below this input parameter, are a series of inputs that could be modified on this Revit element:

![]({{ "/static/images/guides/rir-grasshopper-passthruinputs.png" | prepend: site.baseurl }})

The other side of the component, shows the output parameters as usual. Note that the list of input and output parameters are not always the same. Usually a different set of properties are needed to create and element, and also some of the output properties are calculated based on the computed element (e.g. Walls don't take *Volume* as input but can have that as output). Moreover, not all of the element properties are modifiable through the Revit API:

![]({{ "/static/images/guides/rir-grasshopper-passthruoutputs.png" | prepend: site.baseurl }})

The pass-through components also have an optional output parameter for the type of Revit element that they deals with, in this case {% include ltr/comp.html uuid='b18ef2cc' %}:

![]({{ "/static/images/guides/rir-grasshopper-passthruhidden.gif" | prepend: site.baseurl }})

Now it makes more sense why these components are called pass-through. They pass the input element to the output while making modifications and analysis on it. They also encourage chaining the operations in a series instead of parallel. This is very important to ensure the order of operations since all the target elements are actually owned by Revit and Grasshopper can not determine the full implications of these operations:

![]({{ "/static/images/guides/rir-grasshopper-multiplepassthru.png" | prepend: site.baseurl }})

### Transactional Components

Some of the Revit-aware components need to run *Transactions* on the active document to create new elements or make changes. On each execution of the Grasshopper definition, it is important to know which components contributed to the document changes. This helps understanding and managing the transactions and their implications better (e.g. A developer might change the graph logic to combine many transactional components and improve performance).

These components show a dark background when they execute a transaction:

![]({{ "/static/images/guides/rir-grasshopper-transcomps.png" | prepend: site.baseurl }})

Note that if the input parameters and the target element does not change, the component is not going to make any changes on the next execution of the Grasshopper definition and the component background will change to default gray

You can also use the Grasshopper **Trigger** component, to control when these components are executed:

![]({{ "/static/images/guides/rir-grasshopper-transcompstriggered.png" | prepend: site.baseurl }})

## Previewing Geometry

You can use the toggle preview on Grasshopper components to turn the Revit previews on or off. You can also toggle the preview globally from the *Rhinoceros* tab in Revit:

![]({{ "/static/images/guides/rir-grasshopper-preview.png" | prepend: site.baseurl }})

## Toggling Solver

Grasshopper solver can also be toggled from the *Rhinoceros* tab in Revit. This is especially helpful to reduce wait times on on large Revit models:

![]({{ "/static/images/guides/rir-grasshopper-solver.png" | prepend: site.baseurl }})

## Grasshopper Performance

As mentioned in the sections above, paying close attention to the items below will help increasing the performance of Grasshopper definition:

- Grasshopper runs on top of Revit. So when Revit gets slow (large models, too many open views, ...) Grasshopper might not get the amount of time it needs to update its resources and previews
- Grasshopper previous in Revit require geometry conversions. Having too many previews also slows down the Revit view. Keep the preview on for Grasshopper components when necessary. You can also toggle the preview globally from the *Rhinoceros* tab in Revit
- Running many transactions on individual elements is slower that running one transaction on many elements at once. Try to design the graph logic in a way that a single transactional component can operate on as many elements as you need to modify at once
- Toggling the Grasshopper solver can be helpful in reducing wait times on on large Revit models

## Element Tracking

Element tracking is the ability for Grasshopper components to track which elements they created in a Revit document. Tracking is part of the Revit Project information so even after a Revit project has been closed and reopened, a Grasshopper definition will remember which components are tracking which elements. With this information a component can avoid duplicating previously made elements in Revit.

While most of the time Element tracking will just work, it is important to understanding how to manage Element Tracking in Rhino.inside.Revit as projects move forward.

Only Grasshopper components that create Revit elements will track. Each output on the Grasshopper component does the tracking for thier resulting element. The Grasshopper component will continue to actively update the Revit elements as long as they are tracked and pinned.  Unpining an element in Revit will temporarily pause any additional updates.  By re-pining the element, Grasshopper will continue to update the tracked element.

Only creation components will track their elements.  Other Grasshopper components such Set parameters or modify existing elements will not track.

Named components such as Family definitions and Material will be tracked based on the name of the element. Just like non-named elements, a family or material can be released from Grasshopper components.

Sometimes it is necessary to *release* elements from the tracking Grasshopper component. Once released, a Revit element will no longer be controlled by Grasshopper. This can lead to the element being duplicated if the Grasshopper solution is run in the future. Even if *released* additional modify actions from components later in the definition will continue to make changes to the Elements. Elements can be released in two ways.

1. Right click on the Grasshopper component that created the elements and select Release Elements.
2. All elements in the document can be released by selecting Release elements in the Rhino.Inside.Revit Toolbar.

Another key behavior is when a Grasshopper component that is tracking is deleted. The existing tracked elements from that component must be deleted or *released*.  A Revit warning box wil be displayed when this happen to allow for proper resolution of this issue.  Here are the choices:

1. OK will leave the Elements in the Revit project but they will be released from Grasshopper. The hosted elemente (the floors in this case) are now hosted on normal Revit Levels, but Grasshopper will continue to track the floor themselves. (for instance changing the Floor Type will update those floors.)
1. The Delete All... button will delete the released elements from the Revit project.
1. Cancel will return the Grasshopper component to the canvas and the tracking of those elements will continue.

While affected elements will be higlighted in orange durrin gthe Revit warning box display, the Details >> button can be used to scroll all the objects effected by the delete and specific line items can be selected to highlight the object individually.

To be clear, un-pinning does not release an element if simply pauses any updates that would come from tracking.  Repinning the element will allow Grasshopper to control the element again.

There are new options on the outputs of any create component that relates to Element Tracking.  These options can be accessed by right-clicking on the specific output:

1. Highlight Element This options will highlight the elements tracked by that output.
1. Release Element... will release the elements to Revit and show in Orange those elements released.
1. Unpin Element will unpine the tracked elements.  This will temporarily pause tracking until those elements are re-pinned.
1. Pin Elements will pin the tracked elements and allow Grasshopper to update those elements again.
1. Delete Elements will delete the tracked elements coming from that output. If the deleted elements also host additional elements, those related elements will also be deleted.  The standard Revit resolution dialogs will also appear when this happens.
