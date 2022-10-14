import { nrVary } from "./nrVary";

var nrmLayout = {
    init: () => {
        var domLayout = document.createElement("div");
        domLayout.innerHTML = `
<nav class="navbar fixed-top navbar-expand-lg navbar-dark bg-dark">
  <div class="container-fluid">
    <a class="navbar-brand" href="#">${nrVary.title}</a>
    <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarScroll" aria-controls="navbarScroll" aria-expanded="false" aria-label="Toggle navigation">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarScroll">
      <ul class="navbar-nav me-auto my-2 my-lg-0 navbar-nav-scroll" style="--bs-scroll-height: 100px;">
        <li class="nav-item">
          <a class="nav-link active" aria-current="page" href="#">Home</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="#">Link</a>
        </li>
        <li class="nav-item dropdown">
          <a class="nav-link dropdown-toggle" href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
            Link
          </a>
          <ul class="dropdown-menu">
            <li><a class="dropdown-item" href="#">Action</a></li>
            <li><a class="dropdown-item" href="#">Another action</a></li>
            <li><hr class="dropdown-divider"></li>
            <li><a class="dropdown-item" href="#">Something else here</a></li>
          </ul>
        </li>
        <li class="nav-item">
          <a class="nav-link disabled">Link</a>
        </li>
      </ul>
      <form class="d-flex" role="search">
        <input class="form-control me-2" type="search" placeholder="Search" aria-label="Search">
        <button class="btn btn-outline-success" type="submit">Search</button>
      </form>
    </div>
  </div>
</nav>

<div class="nr-layout-main">
    <div class="nr-layout-left">
        <sl-tree>
            <sl-tree-item>
                Deciduous
                <sl-tree-item>Birch</sl-tree-item>
                <sl-tree-item>
                Maple
                <sl-tree-item>Field maple</sl-tree-item>
                <sl-tree-item>Red maple</sl-tree-item>
                <sl-tree-item>Sugar maple</sl-tree-item>
                </sl-tree-item>
                <sl-tree-item>Oak</sl-tree-item>
            </sl-tree-item>

            <sl-tree-item>
                Coniferous
                <sl-tree-item>Cedar</sl-tree-item>
                <sl-tree-item>Pine</sl-tree-item>
                <sl-tree-item>Spruce</sl-tree-item>
            </sl-tree-item>

            <sl-tree-item>
                Non-trees
                <sl-tree-item>Bamboo</sl-tree-item>
                <sl-tree-item>Cactus</sl-tree-item>
                <sl-tree-item>Fern</sl-tree-item>
            </sl-tree-item>
        </sl-tree>
    </div>
    <div class="nr-layout-right">
        <sl-tab-group class="nr-tabs">
            <sl-tab slot="nav" panel="general">General</sl-tab>
            <sl-tab slot="nav" panel="closable-1" closable>Closable 1</sl-tab>
            <sl-tab slot="nav" panel="closable-2" closable>Closable 2</sl-tab>
            <sl-tab slot="nav" panel="closable-3" closable>Closable 3</sl-tab>

            <sl-tab-panel name="general"></sl-tab-panel>
            <sl-tab-panel name="closable-1">
                <a href="/admin/menu">/admin/menu</a>
                <a href="/admin/log">/admin/log</a>
            </sl-tab-panel>
            <sl-tab-panel name="closable-2"></sl-tab-panel>
            <sl-tab-panel name="closable-3">This is the third closable tab panel.</sl-tab-panel>
        </sl-tab-group>
    </div>
</div>
`;

        document.body.appendChild(domLayout);
    }
}

export { nrmLayout };