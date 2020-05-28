import { Component, OnInit, Inject, Renderer2 } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService, TCareRuleSave, TCareCampaignBasic, TCareCampaignDisplay, TCareCampaignSave, TCareMessageSave } from '../tcare.service';
import { TcareCampaignDialogSequencesComponent } from '../tcare-campaign-dialog-sequences/tcare-campaign-dialog-sequences.component';
import { TcareCampaignDialogRuleComponent } from '../tcare-campaign-dialog-rule/tcare-campaign-dialog-rule.component';

declare var mxGeometry: any;
declare var mxUtils: any;
declare var mxDivResizer: any;
declare var mxClient: any;
declare var mxConstants: any;
declare var mxEditor: any;
declare var mxCell: any;
declare var mxEdgeHandler: any;
declare var mxGuide: any;
declare var mxGraphHandler: any;
declare var mxCodec: any;
declare var mxEvent: any;
declare var mxOutline: any;
declare var mxEdgeStyle: any;
declare var mxCellOverlay: any;
declare var mxImage: any;
declare var mxPerimeter: any;
declare var mxConnectionHandler: any;
declare var mxEffects: any;
declare var mxRectangle: any;
declare var mxWindow: any;
declare var mxConnectionConstraint: any;
declare var mxPoint: any;
declare var mxGraph: any;
declare var mxParallelEdgeLayout: any;
declare var mxLayoutManager: any;

@Component({
  selector: "app-tcare-campaign-create-update",
  templateUrl: "./tcare-campaign-create-update.component.html",
  styleUrls: ["./tcare-campaign-create-update.component.css"],
})
export class TcareCampaignCreateUpdateComponent implements OnInit {
  editorDefind: any;
  id: string;
  offsetY = 30;
  formGroup: FormGroup;
  priority = null;
  title = "Kịch bản";
  campaign: TCareCampaignDisplay;
  cellSave: any;

  constructor(
    private activeRoute: ActivatedRoute,
    private fb: FormBuilder,
    @Inject("BASE_API") private base_url: string,
    private modalService: NgbModal,
    private tcareService: TcareService,
    private router: Router,
    private renderer2: Renderer2
  ) { }

  ngOnInit() {
    this.campaign = new TCareCampaignDisplay();
    this.id = this.activeRoute.params["_value"].id;
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
    });

    this.main(
      document.getElementById("graphContainer"),
      document.getElementById("outlineContainer"),
      document.getElementById("toolbarContainer"),
      document.getElementById("sidebarContainer_sequences"),
      document.getElementById("sidebarContainer_goals"),
      document.getElementById("statusContainer")
    );
  }

  main(container, outline, toolbar, sidebar_sequences, sidebar_goals, status) {
    var that = this;
    if (!mxClient.isBrowserSupported()) {
      // Displays an error message if the browser is not supported.
      mxUtils.error("Browser is not supported!", 200, false);
    } else {
      mxConstants.MIN_HOTSPOT_SIZE = 16;
      mxConstants.DEFAULT_HOTSPOT = 1;
      // Enables guides
      mxGraphHandler.prototype.guidesEnabled = true;
      // Alt disables guides
      mxGuide.prototype.isEnabledForEvent = function (evt) {
        return !mxEvent.isAltDown(evt);
      };

      // Enables snapping waypoints to terminals
      mxEdgeHandler.prototype.snapToTerminals = true;

      // Workaround for Internet Explorer ignoring certain CSS directives
      if (mxClient.IS_QUIRKS) {
        document.body.style.overflow = "hidden";
        new mxDivResizer(container);
        new mxDivResizer(outline);
        new mxDivResizer(toolbar);
        new mxDivResizer(sidebar_sequences);
        new mxDivResizer(sidebar_goals);
        new mxDivResizer(status);
      }

      // Creates a wrapper editor with a graph inside the given container.
      // The editor is used to create certain functionality for the
      // graph, such as the rubberband selection, but most parts
      // of the UI are custom in this example.
      var editor = new mxEditor();
      var graph = editor.graph;
      that.editorDefind = editor;
      graph.setCellsMovable(true);
      graph.setAutoSizeCells(true);
      graph.setPanning(true);
      graph.centerZoom = true;
      graph.panningHandler.useLeftButtonForPanning = true;

      // Disable highlight of cells when dragging from toolbar
      graph.setDropEnabled(false);

      // Uses the port icon while connections are previewed
      mxConnectionHandler.prototype.connectImage = new mxImage(
        that.base_url + "assets/editors/images/connector.gif",
        16,
        16
      );
      // Centers the port icon on the target port
      graph.connectionHandler.targetConnectImage = true;

      // Does not allow dangling edges
      graph.setAllowDanglingEdges(false);

      // Sets the graph container and configures the editor
      editor.setGraphContainer(container);
      var config = mxUtils
        .load("./assets/editors/config/keyhandler-commons.xml")
        .getDocumentElement();
      editor.configure(config);

      var iconTolerance = 20;
      var splash = document.getElementById("splash");
      if (splash != null) {
        try {
          mxEvent.release(splash);
          mxEffects.fadeOut(splash, 100, true);
        } catch (e) {
          // mxUtils is not available (library not loaded)
          splash.parentNode.removeChild(splash);
        }
      }

      var group = new mxCell("Group", new mxGeometry(), "group");
      group.setVertex(false);
      editor.defaultGroup = group;
      editor.groupBorderSize = 20;
      // Disables drag-and-drop into non-swimlanes.
      graph.isValidDropTarget = function (cell, cells, evt) {
        return this.isSwimlane(cell);
      };

      // Disables drilling into non-swimlanes.
      graph.isValidRoot = function (cell) {
        return this.isValidDropTarget(cell);
      };

      // Does not allow selection of locked cells
      graph.isCellSelectable = function (cell) {
        return !this.isCellLocked(cell);
      };

      graph.isValidConnection = function (source, target) {
        const source_id = source.id;
        const target_id = target.id;
        var source_edge_length = 0;
        if (source.edges) {
          source_edge_length = source.edges.length;
        }

        for (let i = 0; i < source_edge_length; i++) {
          if (
            source_id == source.edges[i].source.id &&
            target_id == source.edges[i].target.id
          ) {
            return false;
          }
          if (
            source_id == source.edges[i].target.id &&
            target_id == source.edges[i].source.id
          ) {
            return false;
          }
        }
        var styleSource = this.getModel().getStyle(source);
        var styleTarget = this.getModel().getStyle(target);

        if (styleSource == styleTarget || !styleTarget)
          return false;

        else
          return true;
      }

      // Disables HTML labels for swimlanes to avoid conflict
      // for the event processing on the child cells. HTML
      // labels consume events before underlying cells get the
      // chance to process those events.
      //
      // NOTE: Use of HTML labels is only recommended if the specific
      // features of such labels are required, such as special label
      // styles or interactive form fields. Otherwise non-HTML labels
      // should be used by not overidding the following function.
      // See also: configureStylesheet.
      graph.isHtmlLabel = function (cell) {
        return !this.isSwimlane(cell);
      };

      //Add ContextMenu
      graph.popupMenuHandler.factoryMethod = function (menu, cell, evt) {
        return that.createPopupMenu(editor, graph, menu, cell, evt);
      };

      // Shows a "modal" window when double clicking a vertex.
      graph.dblClick = function (evt, cell) {
        // Do not fire a DOUBLE_CLICK event here as mxEditor will
        // consume the event and start the in-place editor.
        if (
          this.isEnabled() &&
          !mxEvent.isConsumed(evt) &&
          cell != null &&
          this.isCellEditable(cell)
        ) {
          if (this.model.isEdge(cell) || !this.isHtmlLabel(cell)) {
            this.startEditingAtCell(cell);
          }
          else {
            if (cell.style == "sequences") {
              let modalRef = that.modalService.open(TcareCampaignDialogSequencesComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
              modalRef.componentInstance.title = 'Cài đặt';
              modalRef.componentInstance.cell = cell;
              modalRef.result.then(
                () => {
                  that.updateCampaign(graph);
                });
            }
            if (cell.style == "rule") {
              let modalRef = that.modalService.open(TcareCampaignDialogRuleComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
              modalRef.componentInstance.title = 'Cài đặt';
              modalRef.componentInstance.cell = cell;
              modalRef.result.then(
                () => {
                  that.updateCampaign(graph);
                });
            }
            else { return false; }
          }
        }

        // Disables any default behaviour for the double click
        mxEvent.consume(evt);
      };

      // Enables new connections
      graph.setConnectable(true);

      // Adds all required styles to the graph (see below)
      that.configureStylesheet(graph);

      // Adds sidebar icons.
      //
      // NOTE: For non-HTML labels a simple string as the third argument
      // and the alternative style as shown in configureStylesheet should
      // be used. For example, the first call to addSidebar icon would
      // be as follows:
      // that.addSidebarIcon(graph, sidebar, 'Website', './assets/editors/images/icons48/earth.png');

      //add image on hover
      function mxIconSet(value) {
        this.images = [];
        var graph = value.view.graph;
        // Delete
        var img = mxUtils.createImage("./assets/editors/images/delete2.png");
        img.setAttribute("title", "Delete");
        img.style.position = "absolute";
        img.style.cursor = "pointer";
        img.style.width = "16px";
        img.style.height = "16px";
        img.style.left = value.x + value.width - 5 + "px";
        img.style.top = value.y - 10 + "px";

        mxEvent.addGestureListeners(
          img,
          mxUtils.bind(this, function (evt) {
            mxEvent.consume(evt);
          })
        );

        //xóa stage
        mxEvent.addListener(
          img,
          "click",
          mxUtils.bind(this, function (evt) {
            graph.removeCells([value.cell]);
            mxEvent.consume(evt);
            this.destroy();
            that.updateCampaign(graph);
          })
        );
        value.view.graph.container.appendChild(img);
        this.images.push(img);
      }

      mxIconSet.prototype.destroy = function () {
        if (this.images != null) {
          for (var i = 0; i < this.images.length; i++) {
            var img = this.images[i];
            img.parentNode.removeChild(img);
          }
        }
        this.images = null;
      };

      //Mouse listener
      graph.addMouseListener({
        currentState: null,
        currentIconSet: null,
        mouseDown: function (sender, me) {
          // Hides icons on mouse down
          if (this.currentState != null) {
            this.dragLeave(me.getEvent(), this.currentState);
            this.currentState = null;
          }
        },
        mouseMove: function (sender, me) {
          if (
            this.currentState != null &&
            (me.getState() == this.currentState || me.getState() == null)
          ) {
            var tol = iconTolerance;
            var tmp = new mxRectangle(
              me.getGraphX() - tol,
              me.getGraphY() - tol,
              2 * tol,
              2 * tol
            );

            if (mxUtils.intersects(tmp, this.currentState)) {
              return;
            }
          }

          var tmp = graph.view.getState(me.getCell());

          // Ignores everything but vertices
          if (
            graph.isMouseDown ||
            (tmp != null && !graph.getModel().isVertex(tmp.cell))
          ) {
            tmp = null;
          }

          if (tmp != this.currentState) {
            if (this.currentState != null) {
              this.dragLeave(me.getEvent(), this.currentState);
            }

            this.currentState = tmp;

            if (this.currentState != null) {
              this.dragEnter(me.getEvent(), this.currentState);
            }
          }
        },
        mouseUp: function (sender, me) { },
        dragEnter: function (evt, state) {
          if (this.currentIconSet == null) {
            this.currentIconSet = new mxIconSet(state);
          }
        },
        dragLeave: function (evt, state) {
          if (this.currentIconSet != null) {
            this.currentIconSet.destroy();
            this.currentIconSet = null;
          }
        },
      });

      // thêm 1 step
      that.addSidebarIcon(graph, sidebar_sequences,
        '<div style="position: relative; ">' +
        '<img src="./assets/editors/images/steps.png" style="height: 35px; width: 35px; object-fit: cover;">' +
        '<div class="d-flex justify-content-center"><b class="position-absolute mt-1" style="font-size: 0.8rem; white-space: nowrap;">Sequence</b></div>' +
        "</div>",
        "./assets/editors/images/steps.png", "sequences");

      //thêm 1 birthday
      that.addSidebarIcon(graph, sidebar_goals,
        '<div style="position: relative;">' +
        '<img src="./assets/editors/images/birthday.png" style="height: 35px; width: 35px; object-fit: cover;">' +
        '<div class="d-flex justify-content-center"><b class="position-absolute mt-1" style="font-size: 0.8rem; white-space: nowrap;">Điều kiện</b></div>' +
        "</div>",
        "./assets/editors/images/birthday.png", "rule");


      that.addToolbarButton(editor, toolbar, "zoomIn", "", "./assets/editors/images/zoom_in.png", true);
      that.addToolbarButton(editor, toolbar, "zoomOut", "", "./assets/editors/images/zoom_out.png", true);
      that.addToolbarButton(editor, toolbar, "actualSize", "", "./assets/editors/images/view_1_1.png", true);
      that.addToolbarButton(editor, toolbar, "fit", "", "./assets/editors/images/fit_to_size.png", true);
      // Creates the outline (navigator, overview) for moving
      // around the graph in the top, right corner of the window.
      new mxOutline(graph, outline);

      // auto generate start and Task

      if (that.id) {
        this.load(editor);
      }
    }
  }

  updateCampaign(graph) {
    var enc = new mxCodec(mxUtils.createXmlDocument());
    var node = enc.encode(graph.getModel());
    var val = new TCareCampaignSave();
    val.graphXml = mxUtils.getPrettyXml(node);
    val.name = this.campaign.name;
    this.tcareService.update(this.id, val).subscribe(
      () => { }
    );
  }

  load(editor) {
    let that = this;
    this.tcareService.get(this.id).subscribe(
      result => {
        this.campaign = result;
        that.formGroup.get('name').patchValue(result.name);
        if (result.graphXml) {
          editor.graph.getModel().beginUpdate();
          try {
            var xml = result.graphXml;
            var doc = mxUtils.parseXml(xml);
            var dec = new mxCodec(doc);
            dec.decode(doc.documentElement, editor.graph.getModel());
          }
          finally {
            editor.graph.getModel().endUpdate();
          }
        } else {
          var label = '<div style="position: relative;">' +
            '<img src="./assets/editors/images/birthday.png" style="height: 35px; width: 35px; object-fit: cover;">' +
            '<div class="d-flex justify-content-center"><b class="position-absolute mt-1" style="font-size: 0.8rem; white-space: nowrap;">Điều kiện</b></div>' +
            "</div>";
          var parent = editor.graph.getDefaultParent();
          editor.graph.getModel().beginUpdate();
          try {
            var v1 = editor.graph.insertVertex(parent, '', label, 50, 200, 40, 40, 'rule');
            v1.mxTransient.push("name");
            v1.mxTransient.push("beforeDays");
            v1.name = 'rule';
            v1.beforeDays = 0;
          }
          finally {
            editor.graph.getModel().endUpdate();
          }
          editor.graph.setSelectionCell(v1);
        }
      }
    )
  }

  addToolbarButton(editor, toolbar, action, label, image, isTransparent?) {
    var button = document.createElement('button');
    button.style.fontSize = '10';
    if (image != null) {
      var img = document.createElement('img');
      img.setAttribute('src', image);
      img.style.width = '16px';
      img.style.height = '16px';
      img.style.verticalAlign = 'middle';
      img.style.marginRight = '2px';
      button.appendChild(img);
    }
    if (isTransparent) {
      button.style.background = 'transparent';
      button.style.color = '#FFFFFF';
      button.style.border = 'none';
    }
    mxEvent.addListener(button, 'click', function (evt) {
      setTimeout(() => {
        editor.execute(action);
      }, 100);
    });
    mxUtils.write(button, label);
    toolbar.appendChild(button);
  }

  addSidebarIcon(graph, sidebar, label, image, typeShape?) {
    let that = this;
    // Function that is executed when the image is dropped on
    // the graph. The cell argument points to the cell under
    // the mousepointer if there is one.
    var funct = function (graph, evt, cell, x, y) {
      var parent = graph.getDefaultParent();
      var model = graph.getModel();

      var v1 = null;
      var lableRead = '<div style="position: relative;">' +
        '<img src="./assets/editors/images/icons/read-message.png" style="height: 35px; width: 35px; object-fit: cover;">' +
        '<div class="d-flex justify-content-center"><b class="position-absolute mt-1" style="font-size: 0.8rem; white-space: nowrap;">Đã đọc</b></div>' +
        "</div>";
      var lableUnRead = '<div style="position: relative;">' +
        '<img src="./assets/editors/images/icons/unread-message.png" style="height: 35px; width: 35px; object-fit: cover;">' +
        '<div class="d-flex justify-content-center"><b class="position-absolute mt-1" style="font-size: 0.8rem; white-space: nowrap;">Chưa đọc</b></div>' +
        "</div>";
      model.beginUpdate();
      try {
        if (typeShape == "sequences") {

          v1 = graph.insertVertex(parent, '', label, x, y, 110, 40, typeShape);
          v1.mxTransient.push("name");
          v1.mxTransient.push("methodType");
          v1.mxTransient.push("intervalType");
          v1.mxTransient.push("intervalNumber");
          v1.mxTransient.push("sheduleDate");
          v1.mxTransient.push("content");
          v1.mxTransient.push("channelSocialId");
          v1.mxTransient.push("channelType");
          v1.mxTransient.push("tCareCampaignId");
          v1.mxTransient.push("parentId");
          v1.name = typeShape;
          v1.tCareCampaignId = that.id;

          var v_read = graph.insertVertex(parent, '', lableRead, x + 200, y + 50, 40, 40, 'read');
          v_read.mxTransient.push("parentId");
          v_read.mxTransient.push("name");
          v_read.name = 'read'
          graph.insertEdge(parent, '', '', v1, v_read);

          var v_unread = graph.insertVertex(parent, '', lableUnRead, x + 200, y - 50, 40, 40, 'unread');
          v_unread.mxTransient.push("parentId");
          v_unread.mxTransient.push("name");
          v_unread.name = 'unread'
          graph.insertEdge(parent, '', '', v1, v_unread);

        }

        if (typeShape == "rule") {
          v1 = graph.insertVertex(parent, '', label, x, y, 40, 40, typeShape);
          v1.mxTransient.push("name");
          v1.mxTransient.push("beforeDays");
          v1.name = typeShape;
          v1.beforeDays = 0;
        }
      } finally {
        model.endUpdate();
      }
      graph.setSelectionCell(v1);
    }

    // Creates the image which is used as the sidebar icon (drag source)
    var div = that.renderer2.createElement("div");
    that.renderer2.addClass(div, "sidebar-icon");
    var lab = that.renderer2.createElement("label");
    var content = that.renderer2.createText(typeShape);
    that.renderer2.appendChild(lab, content);
    var img = that.renderer2.createElement("img");
    img.setAttribute("src", image);
    img.style.width = "35px";
    img.style.height = "35px";
    img.title = typeShape;
    that.renderer2.appendChild(div, img);
    that.renderer2.appendChild(div, lab);
    sidebar.appendChild(div);
    // When drag Icon image
    var dragImage = div.cloneNode(true);
    var ds = mxUtils.makeDraggable(img, graph, funct, dragImage, 0, 0, true, true);
    ds.setGuidesEnabled(true);
  }

  makeId(length): string {
    var result = '';
    var characters = '1234567890';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
  }

  configureStylesheet(graph) {
    var style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_RECTANGLE;
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
    style[mxConstants.STYLE_GRADIENTCOLOR] = "#41B9F5";
    style[mxConstants.STYLE_FILLCOLOR] = "#8CCDF5";
    style[mxConstants.STYLE_STROKECOLOR] = "#1B78C8";
    style[mxConstants.STYLE_FONTCOLOR] = "#000000";
    style[mxConstants.STYLE_ROUNDED] = true;
    // style[mxConstants.STYLE_OPACITY] = '80';
    style[mxConstants.STYLE_FONTSIZE] = "12";
    style[mxConstants.STYLE_FONTSTYLE] = 0;
    // style[mxConstants.STYLE_IMAGE_WIDTH] = '40';
    // style[mxConstants.STYLE_IMAGE_HEIGHT] = '48';
    graph.getStylesheet().putDefaultVertexStyle(style);

    style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_SWIMLANE;
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_FILLCOLOR] = "#FF9103";
    style[mxConstants.STYLE_GRADIENTCOLOR] = "#F8C48B";
    style[mxConstants.STYLE_STROKECOLOR] = "#E86A00";
    style[mxConstants.STYLE_FONTCOLOR] = "#000000";
    style[mxConstants.STYLE_ROUNDED] = true;
    style[mxConstants.STYLE_OPACITY] = "80";
    style[mxConstants.STYLE_STARTSIZE] = "30";
    style[mxConstants.STYLE_FONTSIZE] = "16";
    style[mxConstants.STYLE_FONTSTYLE] = 1;
    graph.getStylesheet().putCellStyle("group", style);

    style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_IMAGE;
    style[mxConstants.STYLE_FONTCOLOR] = "#774400";
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_PERIMETER_SPACING] = "6";
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_LEFT;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
    style[mxConstants.STYLE_FONTSIZE] = "10";
    style[mxConstants.STYLE_FONTSTYLE] = 2;
    style[mxConstants.STYLE_IMAGE_WIDTH] = "16";
    style[mxConstants.STYLE_IMAGE_HEIGHT] = "16";
    graph.getStylesheet().putCellStyle("port", style);

    style = graph.getStylesheet().getDefaultEdgeStyle();
    style[mxConstants.STYLE_LABEL_BACKGROUNDCOLOR] = "#FFFFFF";
    style[mxConstants.STYLE_STROKEWIDTH] = "2";
    style[mxConstants.STYLE_ROUNDED] = true;
    style[mxConstants.STYLE_EDGE] = mxEdgeStyle.SideToSide;
    graph.alternateEdgeStyle = "elbow=vertical";
  }

  createPopupMenu(editor, graph, menu, cell, evt) {
    var that = this;
    if (cell != null) {
      if (cell.style) {
        menu.addItem("Cut", "./assets/editors/images/cut.png",
          function () {
            editor.execute("cut");
            that.cellSave = cell;
            that.updateCampaign(graph);
          }
        );
        menu.addItem("Copy", "./assets/editors/images/copy.png",
          function () {
            editor.execute("copy");
          }
        );
      }
    } else {
      menu.addItem("Paste", "./assets/editors/images/paste.png", function () {
        if (that.cellSave) {
          editor.execute("paste", that.cellSave, evt);
          that.updateCampaign(graph);
        }

      });
      menu.addItem("Fit", "./assets/editors/images/zoom.gif", function () {
        graph.fit();
      });
      menu.addItem("Actual", "./assets/editors/images/zoomactual.gif",
        function () {
          graph.zoomActual();
        }
      );
    }
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }
    var value = this.formGroup.value;
    var enc = new mxCodec(mxUtils.createXmlDocument());
    var node = enc.encode(this.editorDefind.graph.getModel());
    value.graphXml = mxUtils.getPrettyXml(node);
    value.projectType = "project-process";
    //api update
    if (this.id) {
      this.tcareService.update(this.id, value).subscribe(() => {
        console.log("Thành công");
        this.router.navigateByUrl("tcare-campaigns");
      });
    }
  }
}
