import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService, TCareRuleSave, TCareCampaignBasic, TCareCampaignDisplay, TCareCampaignSave } from '../tcare.service';
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
  selector: 'app-tcare-campaign-create-update',
  templateUrl: './tcare-campaign-create-update.component.html',
  styleUrls: ['./tcare-campaign-create-update.component.css']
})
export class TcareCampaignCreateUpdateComponent implements OnInit {
  editorDefind: any;
  id: string;
  offsetY = 30;
  formGroup: FormGroup;
  priority = null;
  title = "Cài đặt kịch bản";
  campaign: TCareCampaignDisplay;

  constructor(
    private activeRoute: ActivatedRoute,
    private fb: FormBuilder,
    @Inject('BASE_API') private base_url: string,
    private modalService: NgbModal,
    private tcareService: TcareService
  ) { }

  ngOnInit() {
    this.campaign = new TCareCampaignDisplay();
    this.id = this.activeRoute.params['_value'].id;
    this.formGroup = this.fb.group({
      name: ['', Validators.required]
    })

    this.main(
      document.getElementById('graphContainer'),
      document.getElementById('outlineContainer'),
      document.getElementById('toolbarContainer'),
      document.getElementById('sidebarContainer'),
      document.getElementById('statusContainer')
    );
  }

  main(container, outline, toolbar, sidebar, status) {
    var that = this;
    if (!mxClient.isBrowserSupported()) {
      // Displays an error message if the browser is not supported.
      mxUtils.error('Browser is not supported!', 200, false);
    }
    else {
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
        document.body.style.overflow = 'hidden';
        new mxDivResizer(container);
        new mxDivResizer(outline);
        new mxDivResizer(toolbar);
        new mxDivResizer(sidebar);
        new mxDivResizer(status);
      }

      // Creates a wrapper editor with a graph inside the given container.
      // The editor is used to create certain functionality for the
      // graph, such as the rubberband selection, but most parts
      // of the UI are custom in this example.
      var editor = new mxEditor();
      var graph = editor.graph;
      that.editorDefind = editor;
      var model = graph.getModel();
      var parent = graph.getDefaultParent();
      graph.setCellsMovable(true);
      graph.setAutoSizeCells(true);
      graph.setPanning(true);
      graph.centerZoom = true;
      graph.panningHandler.useLeftButtonForPanning = true;

      // Disable highlight of cells when dragging from toolbar
      graph.setDropEnabled(false);

      // Uses the port icon while connections are previewed
      mxConnectionHandler.prototype.connectImage = new mxImage(that.base_url + 'assets/editors/images/connector.gif', 16, 16);
      // Centers the port icon on the target port
      graph.connectionHandler.targetConnectImage = true;

      // Does not allow dangling edges  
      graph.setAllowDanglingEdges(false);

      //defind port


      // Sets the graph container and configures the editor
      editor.setGraphContainer(container);
      var config = mxUtils.load(
        './assets/editors/config/keyhandler-commons.xml').
        getDocumentElement();
      editor.configure(config);

      var iconTolerance = 20;
      var splash = document.getElementById('splash');
      if (splash != null) {
        try {
          mxEvent.release(splash);
          mxEffects.fadeOut(splash, 100, true);
        }
        catch (e) {

          // mxUtils is not available (library not loaded)
          splash.parentNode.removeChild(splash);
        }
      }


      // Defines the default group to be used for grouping. The
      // default group is a field in the mxEditor instance that
      // is supposed to be a cell which is cloned for new cells.
      // The groupBorderSize is used to define the spacing between
      // the children of a group and the group bounds.
      var group = new mxCell('Group', new mxGeometry(), 'group');
      group.setVertex(false);
      group.setConnectable(false);
      editor.defaultGroup = group;
      editor.groupBorderSize = 20;
      // Disables drag-and-drop into non-swimlanes.
      graph.isValidDropTarget = function (cell, cells, evt) {
        return this.isSwimlane(cell);
      };

      // Disables drilling into non-swimlanes.
      graph.isValidRoot = function (cell) {
        return this.isValidDropTarget(cell);
      }

      // Does not allow selection of locked cells
      graph.isCellSelectable = function (cell) {
        return !this.isCellLocked(cell);
      };

      // var previousIsValidSource = graph.isValidSource;
      //disable resource success
      // graph.isValidSource = function (cell) {
      //   if (previousIsValidSource.apply(this, arguments)) {
      //     var style = this.getModel().getStyle(cell);
      //     if (style == "email") {
      //       return style == null || !(style == 'email' || style.indexOf('email') == 0);
      //     }
      //     if (style == "cancel") {
      //       return style == null || !(style == 'cancel' || style.indexOf('cancel') == 0);
      //     }
      //     if (style == "done") {
      //       return style == null || !(style == 'done' || style.indexOf('done') == 0);
      //     }
      //     else {
      //       return true;
      //     }
      //   }
      //   return false;
      // };

      // Start-states are no valid targets, we do not
      // perform a call to the superclass function because
      // this would call isValidSource
      // Note: All states are start states in
      // the example below, so we use the state
      // style below

      graph.isValidConnection = function (source, target) {
        var styleSource = this.getModel().getStyle(source);
        var styleTarget = this.getModel().getStyle(target);
        if (styleSource == styleTarget)
          return false;
        // if ((styleTarget == 'action' && styleSource == 'same-time') || (styleSource == 'action' && styleTarget == 'same-time'))
        //   return false;
        else
          return true;
      }

      // Returns a shorter label if the cell is collapsed and no
      // label for expanded groups
      graph.getLabel = function (cell) {
        var tmp = mxGraph.prototype.getLabel.apply(this, arguments); // "supercall"

        if (this.isCellLocked(cell)) {
          // Returns an empty label but makes sure an HTML
          // element is created for the label (for event
          // processing wrt the parent label)
          return '';
        }
        else if (this.isCellCollapsed(cell)) {
          var index = tmp.indexOf('</h1>');

          if (index > 0) {
            tmp = tmp.substring(0, index + 5);
          }
        }

        return tmp;
      }

      var layout = new mxParallelEdgeLayout(graph);
      var layoutMgr = new mxLayoutManager(graph);

      layoutMgr.getLayout = function (cell) {
        if (cell.getChildCount() > 0) {
          return layout;
        }
      };
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
      }

      // To disable the folding icon, use the following code:

      graph.popupMenuHandler.factoryMethod = function (menu, cell, evt) {
        return that.createPopupMenu(graph, menu, cell, evt);
      };

      // Shows a "modal" window when double clicking a vertex.
      graph.dblClick = function (evt, cell) {
        // Do not fire a DOUBLE_CLICK event here as mxEditor will
        // consume the event and start the in-place editor.
        if (this.isEnabled() &&
          !mxEvent.isConsumed(evt) &&
          cell != null &&
          this.isCellEditable(cell)) {
          if (this.model.isEdge(cell) ||
            !this.isHtmlLabel(cell)) {
            this.startEditingAtCell(cell);
          }
          else {
            return;
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
      function mxIconSet(state) {
        this.images = [];
        var graph = state.view.graph;

        // Icon1
        // var img = mxUtils.createImage('./assets/editors/images/copy.png');
        // img.setAttribute('title', 'Duplicate');
        // img.style.position = 'absolute';
        // img.style.cursor = 'pointer';
        // img.style.width = '16px';
        // img.style.height = '16px';
        // img.style.left = (state.x + state.width) + 'px';
        // img.style.top = (state.y + state.height) + 'px';

        // mxEvent.addGestureListeners(img,
        //   mxUtils.bind(this, function (evt) {
        //     if (state.cell.style == "stage") {
        //       var lableStage = '<div style="position: relative; ">' +
        //         '<img src="./assets/editors/images/steps.png" style="width:121px; background-color: #3ecc67; padding: 10px; border-radius: 13px; height: 120px">' +
        //         '<div style ="position: absolute; text-align: center; width: 120px;"><h5>Giai đoạn</h5></div>' +
        //         '</div>';
        //       var stage = new ProjectTaskTypeSave();
        //       stage.name = "Giai đoạn";
        //       stage.projectIds = [];
        //       stage.projectIds.push(that.id);
        //       that.stageService.create(stage).subscribe(
        //         result => {
        //           var vStage = null;
        //           graph.getModel().beginUpdate()
        //           try {
        //             vStage = graph.insertVertex(parent, result.id, lableStage, 250, 166, 120, 119, 'stage');
        //             vStage.geometry.alternateBounds = new mxRectangle(0, 0, 120, 40);
        //             vStage.mxTransient.push('name');
        //             vStage.name = result.name;
        //             var enc = new mxCodec(mxUtils.createXmlDocument());
        //             var node = enc.encode(graph.getModel());
        //             var value = new ProjectProjectSave();
        //             value.xml = mxUtils.getPrettyXml(node);
        //             value.name = that.formGroup.get('name').value;
        //             value.projectType = "project-process";
        //             that.projectService.update(that.id, value).subscribe(
        //               () => { }
        //             );
        //           } finally {
        //             graph.getModel().endUpdate();
        //           }
        //           graph.setSelectionCell(vStage);
        //         }
        //       )
        //     } else {
        //       var s = graph.gridSize;
        //       graph.setSelectionCells(graph.moveCells([state.cell], s, s, true));
        //       mxEvent.consume(evt);
        //       this.destroy();
        //     }
        //   })
        // );

        // state.view.graph.container.appendChild(img);
        // this.images.push(img);

        // Delete
        var img = mxUtils.createImage('./assets/editors/images/delete2.png');
        img.setAttribute('title', 'Delete');
        img.style.position = 'absolute';
        img.style.cursor = 'pointer';
        img.style.width = '16px';
        img.style.height = '16px';
        img.style.left = (state.x + state.width) + 'px';
        img.style.top = (state.y - 16) + 'px';

        mxEvent.addGestureListeners(img,
          mxUtils.bind(this, function (evt) {
            // Disables dragging the image
            mxEvent.consume(evt);
          })
        );

        //xóa stage 
        mxEvent.addListener(img, 'click',
          mxUtils.bind(this, function (evt) {
            // if (state.cell.style == "stage" || state.cell.style == "done") {
            //   that.stageService.delete(state.cell.id).subscribe(
            //     () => {
            //       var enc = new mxCodec(mxUtils.createXmlDocument());
            //       var node = enc.encode(graph.getModel());
            //       var value = new ProjectProjectSave();
            //       value.xml = mxUtils.getPrettyXml(node);
            //       value.name = that.formGroup.get('name').value;
            //       value.projectType = "project-process";
            //       that.projectService.update(that.id, value).subscribe(
            //         () => { }
            //       );
            //     }
            //   )
            // }
            graph.removeCells([state.cell]);
            mxEvent.consume(evt);
            this.destroy();
          })
        );

        state.view.graph.container.appendChild(img);
        this.images.push(img);
      };

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
      graph.addMouseListener(
        {
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
            if (this.currentState != null && (me.getState() == this.currentState ||
              me.getState() == null)) {
              var tol = iconTolerance;
              var tmp = new mxRectangle(me.getGraphX() - tol,
                me.getGraphY() - tol, 2 * tol, 2 * tol);

              if (mxUtils.intersects(tmp, this.currentState)) {
                return;
              }
            }

            var tmp = graph.view.getState(me.getCell());

            // Ignores everything but vertices
            if (graph.isMouseDown || (tmp != null && !graph.getModel().isVertex(tmp.cell))) {
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
          }
        });

      //thêm 1 birthday
      that.addSidebarIcon(graph, sidebar,
        '<div style="position: relative;">' +
        '<img src="./assets/editors/images/birthday.png" style="height: 70px; width: 70px;">' +
        `<div style ="position:absolute; width: 70px; content:'/a'; white-space: initial;"><h5>Sinh nhật</h5></div>` +
        '</div>',
        './assets/editors/images/birthday.png', 'birthday');

      // thêm 1 step
      that.addSidebarIcon(graph, sidebar,
        '<div style="position: relative; ">' +
        '<img src="./assets/editors/images/steps.png" style="width:121px; background-color: #3ecc67; padding: 10px; border-radius: 13px; height: 120px">' +
        `<div style ="position:absolute; width: 120px; content: '/a'; white-space: initial;"><h5>Sequences</h5></div>` +
        '</div>',
        './assets/editors/images/steps.png', 'sequences');

      //thêm 1 hành động
      // that.addSidebarIcon(graph, sidebar,
      //   '<div style="position: relative;">' +
      //   '<img src="./assets/editors/images/action-play-pic.png" style = "height: 88px; width: 90px;"/>' +
      //   '<div style ="position: absolute; text-align: center;"><h5>Hành động</h5></div>' +
      //   '</div>',
      //   './assets/editors/images/action-play-pic.png', 'action');

      // Creates a new DIV that is used as a toolbar and adds
      // toolbar buttons.
      var spacer = document.createElement('div');
      spacer.style.display = 'inline';
      spacer.style.padding = '8px';

      // that.addToolbarButton(editor, toolbar, 'groupOrUngroup', '(Un)group', './assets/editors/images/group.png');

      // Defines a new action for deleting or ungrouping
      //defind toolbar
      // that.addToolbarButton(editor, toolbar, 'delete', 'Delete', './assets/editors/images/delete2.png');
      // toolbar.appendChild(spacer.cloneNode(true));
      that.addToolbarButton(editor, toolbar, 'cut', 'Cut', './assets/editors/images/cut.png');
      that.addToolbarButton(editor, toolbar, 'copy', 'Copy', './assets/editors/images/copy.png');
      that.addToolbarButton(editor, toolbar, 'paste', 'Paste', './assets/editors/images/paste.png');
      toolbar.appendChild(spacer.cloneNode(true));
      that.addToolbarButton(editor, toolbar, 'undo', '', './assets/editors/images/undo.png');
      that.addToolbarButton(editor, toolbar, 'redo', '', './assets/editors/images/redo.png');
      toolbar.appendChild(spacer.cloneNode(true));
      that.addToolbarButton(editor, toolbar, 'show', 'Show', './assets/editors/images/camera.png');
      that.addToolbarButton(editor, toolbar, 'print', 'Print', './assets/editors/images/printer.png');
      toolbar.appendChild(spacer.cloneNode(true));
      // that.addToolbarButton(editor, toolbar, 'export', 'Save', './assets/editors/images/export1.png');
      // that.addToolbarButton(editor, toolbar, 'import', 'Load', './assets/editors/images/export1.png');

      // Defines a new export action
      editor.addAction('export', function (editor, cell) {
        var textarea = document.createElement('textarea');
        textarea.style.width = '400px';
        textarea.style.height = '400px';
        var enc = new mxCodec(mxUtils.createXmlDocument());
        var node = enc.encode(editor.graph.getModel());
        textarea.value = mxUtils.getPrettyXml(node);
        that.showModalWindow(graph, 'XML', textarea, 410, 440);
      });

      //import a flowchart
      editor.addAction('import', function (editor) {
        that.load(editor)
      });


      // ---

      // Adds toolbar buttons into the status bar at the bottom
      // of the window.
      status.appendChild(spacer.cloneNode(true));

      // that.addToolbarButton(editor, status, 'enterGroup', 'Enter', './assets/editors/images/view_next.png', true);
      // that.addToolbarButton(editor, status, 'exitGroup', 'Exit', './assets/editors/images/view_previous.png', true);

      status.appendChild(spacer.cloneNode(true));

      that.addToolbarButton(editor, status, 'zoomIn', '', './assets/editors/images/zoom_in.png', true);
      that.addToolbarButton(editor, status, 'zoomOut', '', './assets/editors/images/zoom_out.png', true);
      that.addToolbarButton(editor, status, 'actualSize', '', './assets/editors/images/view_1_1.png', true);
      that.addToolbarButton(editor, status, 'fit', '', './assets/editors/images/fit_to_size.png', true);

      // Creates the outline (navigator, overview) for moving
      // around the graph in the top, right corner of the window.
      new mxOutline(graph, outline);

      // auto generate start and Task

      if (that.id) {
        this.load(editor);
      }
    }
  }

  makeid(length): string {
    var result = '';
    var characters = '1234567890';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
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

  showModalWindow(graph, title, content, width, height) {
    var background = document.createElement('div');
    background.style.position = 'absolute';
    background.style.left = '0px';
    background.style.top = '0px';
    background.style.right = '0px';
    background.style.bottom = '0px';
    background.style.background = 'black';
    mxUtils.setOpacity(background, 50);
    document.body.appendChild(background);

    if (mxClient.IS_IE) {
      new mxDivResizer(background);
    }

    var x = Math.max(0, document.body.scrollWidth / 2 - width / 2);
    var y = Math.max(10, (document.body.scrollHeight ||
      document.documentElement.scrollHeight) / 2 - height * 2 / 3);
    var wnd = new mxWindow(title, content, x, y, width, height, false, true);
    wnd.setClosable(true);

    // Fades the background out after after the window has been closed
    wnd.addListener(mxEvent.DESTROY, function (evt) {
      graph.setEnabled(true);
      mxEffects.fadeOut(background, 50, true,
        10, 30, true);
    });

    graph.setEnabled(false);
    graph.tooltipHandler.hide();
    wnd.setVisible(true);
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

      model.beginUpdate();
      try {
        // NOTE: For non-HTML labels the image must be displayed via the style
        // rather than the label markup, so use 'image=' + image for the style.
        // as follows: v1 = graph.insertVertex(parent, null, label,
        // pt.x, pt.y, 120, 120, 'image=' + image);



        // else if (typeShape == "stage") {
        //   //trả về id
        //   //
        //   var stage = new ProjectTaskTypeSave();
        //   stage.name = "Giai đoạn";
        //   stage.projectIds = [];
        //   stage.projectIds.push(that.id);
        //   that.stageService.create(stage).subscribe(
        //     result => {
        //       v1 = graph.insertVertex(parent, result.id, label, x, y, 120, 119, typeShape);
        //       v1.mxTransient.push('name');
        //       v1.mxTransient.push('priority');
        //       v1.name = result.name;
        //       v1.priority = "normal";
        //       v1.geometry.alternateBounds = new mxRectangle(0, 0, 120, 119);
        //       var enc = new mxCodec(mxUtils.createXmlDocument());
        //       var node = enc.encode(graph.getModel());
        //       var value = new ProjectProjectSave();
        //       value.xml = mxUtils.getPrettyXml(node);
        //       value.name = that.formGroup.get('name').value;
        //       value.projectType = "project-process";
        //       that.projectService.update(that.id, value).subscribe(
        //         () => { }
        //       );
        //     });
        // }

        // else if (typeShape == "same-time") {
        //   v1 = graph.insertVertex(parent, that.makeid(9), label, x, y, 60, 60, typeShape);
        //   v1.geometry.alternateBounds = new mxRectangle(0, 0, 60, 60);

        // }

        if (typeShape == "sequences") {
          v1 = graph.insertVertex(parent, that.makeid(9), label, x, y, 120, 119, typeShape);
          v1.mxTransient.push('name');
          v1.name = typeShape;
        }

        if (typeShape == "birthday") {
          var value = new TCareRuleSave();
          value.campaignid = that.id;
          value.type = typeShape;
          that.tcareService.createTCareRule(value).subscribe(
            result => {
              v1 = graph.insertVertex(parent, result.id, label, x, y, 70, 70, typeShape);
              v1.mxTransient.push('name');
              v1.name = typeShape;
              var enc = new mxCodec(mxUtils.createXmlDocument());
              var node = enc.encode(graph.getModel());
              var val = new TCareCampaignSave();
              val.graphXml = mxUtils.getPrettyXml(node);
              val.name = that.campaign.name;
              that.tcareService.update(that.id, val).subscribe(
                () => { }
              );
            }
          )
        }
      }
      finally {
        model.endUpdate();
      }

      graph.setSelectionCell(v1);
    }

    // Creates the image which is used as the sidebar icon (drag source)
    var img = document.createElement('img');
    img.setAttribute('src', image);
    img.style.width = '48px';
    img.style.height = '48px';
    img.style.margin = "16px";
    img.title = typeShape;
    sidebar.appendChild(img);

    var dragElt = document.createElement('div');
    dragElt.style.border = 'dashed black 1px';
    dragElt.style.width = '120px';
    dragElt.style.height = '120px';

    // Creates the image which is used as the drag icon (preview)
    var ds = mxUtils.makeDraggable(img, graph, funct, dragElt, 0, 0, true, true);
    ds.setGuidesEnabled(true);
  }

  configureStylesheet(graph) {
    var style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_RECTANGLE;
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
    style[mxConstants.STYLE_GRADIENTCOLOR] = '#41B9F5';
    style[mxConstants.STYLE_FILLCOLOR] = '#8CCDF5';
    style[mxConstants.STYLE_STROKECOLOR] = '#1B78C8';
    style[mxConstants.STYLE_FONTCOLOR] = '#000000';
    style[mxConstants.STYLE_ROUNDED] = true;
    style[mxConstants.STYLE_OPACITY] = '80';
    style[mxConstants.STYLE_FONTSIZE] = '12';
    style[mxConstants.STYLE_FONTSTYLE] = 0;
    style[mxConstants.STYLE_IMAGE_WIDTH] = '48';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '48';
    graph.getStylesheet().putDefaultVertexStyle(style);

    style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_SWIMLANE;
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_FILLCOLOR] = '#FF9103';
    style[mxConstants.STYLE_GRADIENTCOLOR] = '#F8C48B';
    style[mxConstants.STYLE_STROKECOLOR] = '#E86A00';
    style[mxConstants.STYLE_FONTCOLOR] = '#000000';
    style[mxConstants.STYLE_ROUNDED] = true;
    style[mxConstants.STYLE_OPACITY] = '80';
    style[mxConstants.STYLE_STARTSIZE] = '30';
    style[mxConstants.STYLE_FONTSIZE] = '16';
    style[mxConstants.STYLE_FONTSTYLE] = 1;
    graph.getStylesheet().putCellStyle('group', style);

    style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_IMAGE;
    style[mxConstants.STYLE_FONTCOLOR] = '#774400';
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_PERIMETER_SPACING] = '6';
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_LEFT;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
    style[mxConstants.STYLE_FONTSIZE] = '10';
    style[mxConstants.STYLE_FONTSTYLE] = 2;
    style[mxConstants.STYLE_IMAGE_WIDTH] = '16';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '16';
    graph.getStylesheet().putCellStyle('port', style);

    style = graph.getStylesheet().getDefaultEdgeStyle();
    style[mxConstants.STYLE_LABEL_BACKGROUNDCOLOR] = '#FFFFFF';
    style[mxConstants.STYLE_STROKEWIDTH] = '2';
    style[mxConstants.STYLE_ROUNDED] = true;
    style[mxConstants.STYLE_EDGE] = mxEdgeStyle.SideToSide;
    graph.alternateEdgeStyle = 'elbow=vertical';
  }

  createPopupMenu(graph, menu, cell, evt) {
    var that = this;
    if (cell != null) {

      if (cell.style == "sequences") {
        menu.addItem('Cài đặt', './assets/editors/images/icons/settings.png', function () {
          // let modalRef = that.modalService.open(MxgraphDiagramSetupComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
          // modalRef.componentInstance.title = 'Cài đặt';
          // modalRef.componentInstance.cell = cell;
          // modalRef.result.then(
          //   result => {
          //     if (result && result.name) {
          //       graph.getModel().beginUpdate();
          //       try {
          //         var model = new ProjectTaskTypeSave();
          //         model.name = result.name;
          //         model.teamId = result.teamId;
          //         model.userId = result.userId;
          //         if (!model.projectIds) {
          //           model.projectIds = [];
          //         }
          //         model.projectIds.push(that.id);
          //         that.stageService.update(cell.id, model).subscribe(
          //           () => {
          //             cell.value = '<div style="position: relative; ">' +
          //               '<img src="./assets/editors/images/steps.png" style="width:121px; background-color: #3ecc67; padding: 10px; border-radius: 13px; height: 120px">' +
          //               `<div style ="position:absolute; width: 120px; content: '/a'; white-space: initial;"><h5>${result.name}</h5></div>` +
          //               '</div>';
          //             cell.name = result.name;
          //             cell.priority = result.priority;
          //             var overlays = graph.getCellOverlays(cell);
          //             if (overlays) {
          //               graph.removeCellOverlays(cell);
          //             }
          //             if (result.priority == "hight") {
          //               var overlay = new mxCellOverlay(
          //                 new mxImage(`${that.base_url}/assets/editors/images/icons/star.png`, 20, 20),
          //                 'độ ưu tiên cao', 'right', 'top');

          //               // Sets the overlay for the cell in the graph
          //               graph.addCellOverlay(cell, overlay);
          //             } else {
          //               var overlay = new mxCellOverlay(
          //                 new mxImage(`${that.base_url}/assets/editors/images/icons/dot.png`, 1, 1),
          //                 '', 'right', 'top');

          //               // Sets the overlay for the cell in the graph
          //               graph.addCellOverlay(cell, overlay);
          //             }
          //           }
          //         )
          //       } finally {
          //         graph.getModel().endUpdate();
          //       }
          //     }

          //   }
          // )

        });
        // menu.addItem('Thêm hành động', './assets/editors/images/icons/action-play.png', function () {
        //   that.addChild(graph, cell, cell.style, 'add-action');
        // });
        // menu.addItem('Thêm hành động đồng thời', './assets/editors/images/icons/plus-icon.png', function () {
        //   that.addChild(graph, cell, cell.style, 'add-sametime');
        // });

        //dấu gạch ngang
        // menu.addSeparator();
        menu.addItem('Add condition', './assets/editors/images/icons/action-mail.png', function () {
          mxUtils.alert('Hành động');
        });
      }
      if (cell.style == "birthday") {
        menu.addItem('Cài đặt', './assets/editors/images/icons/settings.png', function () {
          let modalRef = that.modalService.open(TcareCampaignDialogRuleComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
          modalRef.componentInstance.title = 'Cài đặt';
          modalRef.componentInstance.cell = cell;
          modalRef.result.then(
            result => {

            });
        })
        menu.addItem('Add Sequences', './assets/editors/images/icons/stage-icon.png', function () {
          that.addChild(graph, cell, cell.style, 'add-stage');
        });
      }
    } else {
      menu.addItem('Fit', './assets/editors/images/zoom.gif', function () {
        graph.fit();
      });

      menu.addItem('Actual', './assets/editors/images/zoomactual.gif', function () {
        graph.zoomActual();
      });
    }
    // menu.addSeparator();
    // menu.addItem('MenuItem3', '../src/images/warning.gif', function () {
    //   mxUtils.alert('MenuItem3: ' + graph.getSelectionCount() + ' selected');
    // });
  }

  addChild(graph, cell, typeShape?, action?) {
    var that = this;
    var model = graph.getModel();
    var parent = graph.getDefaultParent();
    var vertex;
    // var labelAction = '<div style="position: relative;">' +
    //   '<img src="./assets/editors/images/action-play-pic.png" style = "height: 88px; width: 90px;"/>' +
    //   '<div style ="position: absolute; width: 90px;top: 77px; text-align: center;"><h5>Hành động</h5></div>' +
    //   '</div>';
    var lableStage = '<div style="position: relative; ">' +
      '<img src="./assets/editors/images/steps.png" style="width:121px; background-color: #3ecc67; padding: 10px; border-radius: 13px; height: 120px">' +
      '<div style ="position: absolute; text-align: center; width: 120px;"><h5>Giai đoạn</h5></div>' +
      '</div>';

    // var lableSameTime = '<div style="position: relative;">' +
    //   '<img src="./assets/editors/images/plus.png" style="height: 50px; width: 50px;">' +
    //   '<div style ="position: absolute; text-align: center; top:58px; left: -14px; width: 60px;"><h5>Đồng thời</h5></div>' +
    //   '</div>';


    var vertexAction;
    model.beginUpdate();
    try {
      // if (action == "add-action") {
      //   vertex = graph.insertVertex(parent, null, labelAction, cell.geometry.x + 200, cell.geometry.y + that.offsetY, 90, 60, 'action');
      //   var edge = graph.insertEdge(parent, null, '', cell, vertex);
      //   edge.geometry.x = 100;
      //   edge.geometry.y = 0;
      //   edge.geometry.offset = new mxPoint(0, -20);
      // }

      // if (action == "add-sametime") {
      //   vertex = graph.insertVertex(parent, that.makeid(9), lableSameTime, cell.geometry.x + 200, cell.geometry.y + that.offsetY, 90, 60, 'same-time');
      //   var edge = graph.insertEdge(parent, that.makeid(9), '', cell, vertex);
      //   edge.geometry.x = 1;
      //   edge.geometry.y = 0;
      //   edge.geometry.offset = new mxPoint(0, -20);
      // }

      // if (action == "add-stage") {
      //   vertexAction = graph.insertVertex(parent, that.makeid(9), labelAction, cell.geometry.x + 220, cell.geometry.y + that.offsetY, 90, 60, 'action')
      //   var edgeAction = graph.insertEdge(parent, that.makeid(9), '', cell, vertexAction);
      //   edgeAction.geometry.x = 1;
      //   edgeAction.geometry.y = 0;
      //   edgeAction.geometry.offset = new mxPoint(0, -20);

      //   var stage = new ProjectTaskTypeSave();
      //   stage.name = "Giai đoạn";
      //   stage.projectIds = [];
      //   stage.projectIds.push(that.id);
      //   that.stageService.create(stage).subscribe(
      //     result => {
      //       vertex = graph.insertVertex(parent, result.id, cell.value, cell.geometry.x + 400, cell.geometry.y, cell.geometry.width, cell.geometry.height, typeShape);
      //       vertex.mxTransient.push('name');
      //       vertex.mxTransient.push('priority');
      //       vertex.name = result.name;
      //       vertex.priority = "normal";
      //       vertex.geometry.alternateBounds = new mxRectangle(0, 0, 120, 119);

      //       var edge = graph.insertEdge(parent, that.makeid(9), '', vertexAction, vertex);
      //       edge.geometry.x = 1;
      //       edge.geometry.y = 0;
      //       edge.geometry.offset = new mxPoint(0, -20);

      //       var enc = new mxCodec(mxUtils.createXmlDocument());
      //       var node = enc.encode(graph.getModel());
      //       var value = new ProjectProjectSave();
      //       value.xml = mxUtils.getPrettyXml(node);
      //       value.name = that.formGroup.get('name').value;
      //       value.projectType = "project-process";
      //       that.projectService.update(that.id, value).subscribe(
      //         () => { }
      //       );
      //     });


      // }

      // if (action == "action-stage") {
      //   var stage = new ProjectTaskTypeSave();
      //   stage.name = "Giai đoạn";
      //   stage.projectIds = [];
      //   stage.projectIds.push(that.id);
      //   that.stageService.create(stage).subscribe(
      //     result => {
      //       vertex = graph.insertVertex(parent, result.id, lableStage, cell.geometry.x + 200, cell.geometry.y - that.offsetY, 120, 119, 'stage');
      //       vertex.mxTransient.push('name');
      //       vertex.mxTransient.push('priority');
      //       vertex.name = result.name;
      //       vertex.priority = "normal";
      //       vertex.geometry.alternateBounds = new mxRectangle(0, 0, 120, 119);

      //       var edge = graph.insertEdge(parent, that.makeid(9), '', cell, vertex);
      //       edge.geometry.x = 1;
      //       edge.geometry.y = 0;
      //       edge.geometry.offset = new mxPoint(0, -20);

      //       var enc = new mxCodec(mxUtils.createXmlDocument());
      //       var node = enc.encode(graph.getModel());
      //       var value = new ProjectProjectSave();
      //       value.xml = mxUtils.getPrettyXml(node);
      //       value.name = that.formGroup.get('name').value;
      //       value.projectType = "project-process";
      //       that.projectService.update(that.id, value).subscribe(
      //         () => { }
      //       );
      //     });


      // }

      // addOverlays(graph, vertex, true);
    }
    finally {
      model.endUpdate();
    }

    return vertex;
  }

  onSave() {
    if (this.formGroup.invalid) {
      return false;
    }
    var value = this.formGroup.value;
    var enc = new mxCodec(mxUtils.createXmlDocument());
    var node = enc.encode(this.editorDefind.graph.getModel());
    value.xml = mxUtils.getPrettyXml(node);
    value.projectType = 'project-process';
    //api create
  }

}
