import { Component, OnInit, Inject, Renderer2 } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { TcareService, TCareCampaignDisplay, TCareMessageDisplay, TCareRule, TCareRuleCondition } from '../tcare.service';
import { TcareCampaignDialogSequencesComponent } from '../tcare-campaign-dialog-sequences/tcare-campaign-dialog-sequences.component';
import { TcareCampaignDialogRuleComponent } from '../tcare-campaign-dialog-rule/tcare-campaign-dialog-rule.component';

import { NotificationService } from '@progress/kendo-angular-notification';
import { DatePipe } from '@angular/common';
import { TcareCampaignDialogMessageComponent } from '../tcare-campaign-dialog-message/tcare-campaign-dialog-message.component';
import { PartnerCategoryBasic } from 'src/app/partner-categories/partner-category.service';
import { IntlService } from '@progress/kendo-angular-intl';
import * as xml2js from 'xml2js';

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
declare var mxImage: any;
declare var mxPerimeter: any;
declare var mxConnectionHandler: any;
declare var mxEffects: any;
declare var mxRectangle: any;
declare var mxKeyHandler: any;

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
  doc: any;
  constructor(
    private activeRoute: ActivatedRoute,
    private fb: FormBuilder,
    @Inject("BASE_API") private base_url: string,
    private modalService: NgbModal,
    private tcareService: TcareService,
    private renderer2: Renderer2,
    private notificationService: NotificationService,
    private intlService: IntlService
  ) { }

  ngOnInit() {
    this.campaign = new TCareCampaignDisplay();
    this.id = this.activeRoute.params["_value"].id;
    this.formGroup = this.fb.group({
      name: ["", Validators.required],
    });
    this.doc = mxUtils.createXmlDocument();
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

      //create obj
      var sequence = that.doc.createElement('sequence');
      sequence.setAttribute('campaignId', that.id);
      sequence.setAttribute('label', 'Gửi tin');

      var rule = that.doc.createElement('rule');
      rule.setAttribute('logic', 'and');
      rule.setAttribute('label', 'Điều kiện');

      var editor = new mxEditor();
      var graph = editor.graph;
      that.editorDefind = editor;
      graph.setCellsMovable(true);
      graph.setAutoSizeCells(true);
      graph.setPanning(true);
      graph.centerZoom = true;
      graph.panningHandler.useLeftButtonForPanning = true;

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
      // var config = mxUtils
      //   .load("./assets/editors/config/keyHandler-commons.xml")
      //   .getDocumentElement();
      // editor.configure(config);

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


      //validate Connection
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
        if (styleSource == 'read' || styleSource == 'unread' || styleTarget == 'read' || styleTarget == 'unread')
          return false;
        else {
          var value = target.getValue();
          value.setAttribute('parent', source.id);
          graph.getModel().setValue(target, value);
          return true;
        }
      }

      //Add ContextMenu
      graph.popupMenuHandler.factoryMethod = function (menu, cell, evt) {
        return that.createPopupMenu(editor, graph, menu, cell, evt);
      };

      //defind NodeName
      graph.convertValueToString = function (cell) {
        if (mxUtils.isNode(cell.value)) {
          if (cell.value.nodeName.toLowerCase() == 'rule') {
            return 'Điều kiện';
          }

          if (cell.value.nodeName.toLowerCase() == 'sequence') {
            return 'Gửi tin';
          }

          if (cell.value.nodeName.toLowerCase() == 'addtag') {
            return 'Gán nhãn';
          }

          if (cell.value.nodeName.toLowerCase() == 'messageopen') {
            return cell.value.getAttribute('label', '');
          }

          return cell.value.getAttribute('label', '');
        }

        return '';
      };

      //double click Action
      graph.dblClick = function (evt, cell) {
        if (this.isEnabled() && !mxEvent.isConsumed(evt) && cell != null) {
          if (cell.value.nodeName.toLowerCase() == 'sequence') {
            that.popupSequence(graph, cell);
          }

          if (cell.value.nodeName.toLowerCase() == 'rule') {
            that.popupRule(graph, cell);
          }

          if (cell.value.nodeName.toLowerCase() == 'addtag') {
            that.popupAddTag(graph, cell);
          }

          else { return false; }
        }
        // Disables any default behaviour for the double click
        mxEvent.consume(evt);
      };

      // Enables new connections
      graph.setConnectable(true);

      // Adds all required styles to the graph (see below)
      that.configureStylesheet(graph);

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
            if (value.cell.isRoot) {
              alert('bạn không được xóa gốc của sơ đồ');
              return false
            }
            graph.removeCells([value.cell]);
            mxEvent.consume(evt);
            this.destroy();
            if (value.cell.style == 'read') {
              var cellParent = graph.getModel().getCell(value.cell.value.getAttribute('parent'));
              var valueCell = cellParent.getValue();
              valueCell.removeAttribute('messageReadId');
            }
            if (value.cell.style == 'unread') {
              var cellParent = graph.getModel().getCell(value.cell.value.getAttribute('parent'));
              var valueCell = cellParent.getValue();
              valueCell.removeAttribute('messageUnreadId');
            }

            that.onSave();
          })
        );
        value.view.graph.container.appendChild(img);
        this.images.push(img);
      }

      //remove all when remove node
      mxIconSet.prototype.destroy = function () {
        if (this.images != null) {
          for (var i = 0; i < this.images.length; i++) {
            var img = this.images[i];
            img.parentNode.removeChild(img);
          }
        }
        this.images = null;
      };

      var keyHandler = new mxKeyHandler(graph);
      keyHandler.bindKey(46, function (evt) {
        if (graph.isEnabled()) {
          graph.removeCells();
        }
      });

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

      // thêm 1 sequence
      that.addSidebarIcon(graph, sidebar_sequences, sequence, "./assets/editors/images/message-setting.png", "sequence");

      //thêm 1 rule
      that.addSidebarIcon(graph, sidebar_goals, rule, "./assets/editors/images/rule.png", "rule");

      //outLine
      new mxOutline(graph, outline);

      //load Xml
      this.load(editor);
    }
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
          // var rule = that.doc.createElement('rule');
          // rule.setAttribute('logic', 'and');
          // var parent = editor.graph.getDefaultParent();
          // editor.graph.getModel().beginUpdate();
          // try {
          //   var v1 = editor.graph.insertVertex(parent, null, rule, 50, 200, 40, 40, 'rule');
          //   v1.mxTransient.push("name");
          //   v1.name = 'rule';
          //   v1.isRoot = true;
          // }
          // finally {
          //   editor.graph.getModel().endUpdate();
          // }
          // editor.graph.setSelectionCell(v1);
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

  addSidebarIcon(graph, sidebar, obj, image, typeShape?) {
    let that = this;
    // Function that is executed when the image is dropped on
    // the graph. The cell argument points to the cell under
    // the mousepointer if there is one.
    var funct = function (graph, evt, cell, x, y) {
      var parent = graph.getDefaultParent();
      var model = graph.getModel();

      //Chỉ cho phép kéo 1 điều kiện và 1 gửi tin
      var vertices = model.filterDescendants(function (c) {
        return graph.model.isVertex(c);
      });

      if (typeShape == 'sequence') {
        var sequence_cells = model.filterCells(vertices, function (c) {
          return c.value.nodeName.toLowerCase() == 'sequence';
        });

        if (sequence_cells.length) {
          that.notificationService.show({
            content: 'Không thể kéo nhiều gửi tin',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'error', icon: true }
          });

          return false;
        }
      }

      if (typeShape == 'rule') {
        var rule_cells = model.filterCells(vertices, function (c) {
          return c.value.nodeName.toLowerCase() == 'rule';
        });

        if (rule_cells.length) {
          that.notificationService.show({
            content: 'Không thể kéo nhiều điều kiện',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'error', icon: true }
          });

          return false;
        }
      }

      var v1 = null;
      model.beginUpdate();
      try {
        if (typeShape == "sequence") {
          v1 = graph.insertVertex(parent, null, obj, x, y, 110, 50, typeShape);
        }

        if (typeShape == "rule") {
          v1 = graph.insertVertex(parent, null, obj, x, y, 40, 40, typeShape);
        }
      } finally {
        model.endUpdate();
      }

      graph.setSelectionCell(v1);
    }

    // Creates the image which is used as the sidebar icon (drag source)
    var div = that.renderer2.createElement("div");
    that.renderer2.addClass(div, "sidebar-icon");
    // var lab = that.renderer2.createElement("label");
    // var content = that.renderer2.createText(typeShape);
    // that.renderer2.appendChild(lab, content);
    var img = that.renderer2.createElement("img");
    img.setAttribute("src", image);
    img.style.width = "35px";
    img.style.height = "35px";
    // img.title = typeShape;
    that.renderer2.appendChild(div, img);
    // that.renderer2.appendChild(div, lab);
    sidebar.appendChild(div);
    // When drag Icon image
    var dragImage = div.cloneNode(true);
    var ds = mxUtils.makeDraggable(img, graph, funct, dragImage, 0, 0, true, true);
    ds.setGuidesEnabled(true);



  }

  configureStylesheet(graph) {
    var style = new Object();
    let that = this;
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

    //style for sequence
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = '#000000';
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] = that.base_url + 'assets/editors/images/message-setting.png';
    style[mxConstants.STYLE_IMAGE_WIDTH] = '40';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '40';
    style[mxConstants.STYLE_SPACING_TOP] = '55';
    style[mxConstants.STYLE_SPACING] = '0';
    graph.getStylesheet().putCellStyle('sequence', style);

    //style for rule
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = '#000000';
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] = that.base_url + 'assets/editors/images/rule.png';
    style[mxConstants.STYLE_IMAGE_WIDTH] = '40';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '40';
    style[mxConstants.STYLE_SPACING_TOP] = '55';
    style[mxConstants.STYLE_SPACING] = '-8';
    graph.getStylesheet().putCellStyle('rule', style);

    //style for message-read
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = '#000000';
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] = that.base_url + 'assets/editors/images/message-tag-open.png';
    style[mxConstants.STYLE_IMAGE_WIDTH] = '35';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '35';
    style[mxConstants.STYLE_SPACING_TOP] = '50';
    style[mxConstants.STYLE_SPACING] = '-2.5';
    graph.getStylesheet().putCellStyle('read', style);

    //style for message-unread
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = '#000000';
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] = that.base_url + 'assets/editors/images/message-tag.png';
    style[mxConstants.STYLE_IMAGE_WIDTH] = '35';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '35';
    style[mxConstants.STYLE_SPACING_TOP] = '50';
    style[mxConstants.STYLE_SPACING] = '-2.5';
    graph.getStylesheet().putCellStyle('unread', style);

  }

  popupAddTag(graph, cell) {
    let self = this;
    var value = cell.getValue();

    var serializer = new XMLSerializer();
    var valueXMl = serializer.serializeToString(value);

    xml2js.parseString(valueXMl, function (err, result) {
      if (!err) {
        var addTag = result.addTag;
        var item = new Object();
        item['tags'] = [];

        if (addTag.tag) {
          addTag.tag.forEach(t => {
            item['tags'].push(Object.assign({}, t.$));
          });
        }

        let modalRef = self.modalService.open(TcareCampaignDialogMessageComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
        modalRef.componentInstance.item = item;

        modalRef.result.then((result: any) => {
          graph.getModel().beginUpdate();
          try {
            var doc = mxUtils.createXmlDocument();
            var userObject = doc.createElement('addTag');

            result.tags.forEach(t => {
              var tagEl = doc.createElement('tag');
              for (var p in t) {
                if (p == 'id' || p == 'name') {
                  tagEl.setAttribute(p, t[p]);
                }
              }

              userObject.appendChild(tagEl);
            });

            graph.getModel().setValue(cell, userObject);
          }
          finally {
            graph.getModel().endUpdate();
            self.onSave();
          }
        }, () => {
        });
      }
    });
  }

  popupRule(graph, cell) {
    let that = this;
    var value = cell.getValue();

    var serializer = new XMLSerializer();
    var valueXMl = serializer.serializeToString(value);

    xml2js.parseString(valueXMl, function (err, result) {
      if (!err) {
        var rule = result.rule;
        var a = Object.assign({}, rule.$);
        a.conditions = [];
        if (rule.condition) {
          rule.condition.forEach(con => {
            a.conditions.push(Object.assign({}, con.$));
          });
        }
        let modalRef = that.modalService.open(TcareCampaignDialogRuleComponent, { size: 'lg', windowClass: 'o_technical_modal', backdrop: 'static', keyboard: false });
        modalRef.componentInstance.title = 'Cài đặt điều kiện';
        modalRef.componentInstance.audience_filter = a;
        modalRef.result.then((result: any) => {
          graph.getModel().beginUpdate();
          try {
            var doc = mxUtils.createXmlDocument();
            var userObject = doc.createElement('rule');
            for (var p in result) {
              if (p != 'conditions') {
                userObject.setAttribute(p, result[p]);
              }
            }

            result.conditions.forEach(con => {
              var conEl = doc.createElement('condition');
              for (var p in con) {
                conEl.setAttribute(p, con[p]);
              }

              userObject.appendChild(conEl);
            });

            graph.getModel().setValue(cell, userObject);
          }
          finally {
            graph.getModel().endUpdate();
            that.onSave();
          }
        }, () => {
        });
      }
    });
  }

  popupSequence(graph, cell) {
    let that = this;
    var value = cell.getValue();
    var objx = new Object();
    for (let i = 0; i < value.attributes.length; i++) {
      var attribute = value.attributes[i];
      objx[attribute.name] = attribute.nodeValue;
    }

    let modalRef = that.modalService.open(TcareCampaignDialogSequencesComponent, { size: 'lg', windowClass: 'o_technical_modal', scrollable: true, backdrop: 'static', keyboard: false });
    modalRef.componentInstance.title = 'Cài đặt gửi tin';
    modalRef.componentInstance.model = objx;

    modalRef.result.then((result: any) => {
      graph.getModel().beginUpdate();
      try {
        var doc = mxUtils.createXmlDocument();
        var userObject = doc.createElement('sequence');
        for (var p in result) {
          userObject.setAttribute(p, result[p]);
        }

        graph.getModel().setValue(cell, userObject);
      } finally {
        graph.getModel().endUpdate();
        that.onSave();
      }
    }, () => {
    });
  }

  createPopupMenu(editor, graph, menu, cell, evt) {
    var that = this;
    var parent = graph.getDefaultParent();
    if (cell != null) {
      if (cell.style == 'sequence') {
        menu.addItem("Gán nhãn đã đọc", "./assets/editors/images/icons/message-tag-open-icon.png", function () {
          //Nếu có edge messageOpen từ cell này thì sẽ ko cho add thêm nữa
          var model = graph.getModel();
          //tìm tất cả edge của root
          var all_edges = model.filterDescendants(function (c) {
            return graph.model.isEdge(c);
          });

          //lọc edge kết nối vơi cell (source là cellId)
          var connect_edges = model.filterCells(all_edges, function (c) {
            return c.source.id == cell.id && c.value.nodeName.toLowerCase() == 'messageopen';
          });

          if (connect_edges.length) {
            that.notificationService.show({
              content: 'Đã thêm gán nhãn đã đọc',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'error', icon: true }
            });
          } else {
            graph.getModel().beginUpdate();
            try {
              var addTagEl = that.doc.createElement('addTag');
              addTagEl.setAttribute('label', 'Gán nhãn');

              var addTagVertex = graph.insertVertex(parent, null, addTagEl, cell.geometry.x + 200, cell.geometry.y - 50, 40, 40, 'read');

              var messageOpenEl = that.doc.createElement('messageOpen');
              messageOpenEl.setAttribute('label', 'Đã đọc');
              messageOpenEl.setAttribute('name', 'message_open');
              graph.insertEdge(parent, null, messageOpenEl, cell, addTagVertex);
            }
            finally {
              graph.getModel().endUpdate();
            }

            that.onSave();
          }
        });

        menu.addItem("Gán nhãn đã nhận", "./assets/editors/images/icons/message-tag-icon.png", function () {
          //Nếu có edge messageDelivered từ cell này thì sẽ ko cho add thêm nữa
          var model = graph.getModel();
          var all_edges = model.filterDescendants(function (c) {
            return graph.model.isEdge(c);
          });

          var connect_edges = model.filterCells(all_edges, function (c) {
            return c.source.id == cell.id && c.value.nodeName.toLowerCase() == 'messagedelivered';
          });

          if (connect_edges.length) {
            that.notificationService.show({
              content: 'Đã thêm gán nhãn đã nhận',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'error', icon: true }
            });
          } else {
            graph.getModel().beginUpdate();
            try {
              var addTagEl = that.doc.createElement('addTag');
              addTagEl.setAttribute('label', 'Gán nhãn');

              var addTagVertex = graph.insertVertex(parent, null, addTagEl, cell.geometry.x + 200, cell.geometry.y + 50, 40, 40, 'unread');

              var messageDeliveredEl = that.doc.createElement('messageDelivered');
              messageDeliveredEl.setAttribute('label', 'Đã nhận');
              messageDeliveredEl.setAttribute('name', 'message_delivered');
              graph.insertEdge(parent, null, messageDeliveredEl, cell, addTagVertex);
            }
            finally {
              graph.getModel().endUpdate();
            }

            that.onSave();
          }
        })
      }
    } else {
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
    if (this.id) {
      this.tcareService.update(this.id, value).subscribe(() => {
        this.notificationService.show({
          content: 'Thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      });
    }
  }
}
