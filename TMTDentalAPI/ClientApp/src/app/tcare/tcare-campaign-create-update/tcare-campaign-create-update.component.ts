import { Component, OnInit, Inject, Renderer2 } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import {
  TcareService,
  TCareCampaignDisplay,
  TCareMessageDisplay,
  TCareRule,
  TCareRuleCondition,
} from "../tcare.service";
import { TcareCampaignDialogSequencesComponent } from "../tcare-campaign-dialog-sequences/tcare-campaign-dialog-sequences.component";
import { TcareCampaignDialogRuleComponent } from "../tcare-campaign-dialog-rule/tcare-campaign-dialog-rule.component";

import { NotificationService } from "@progress/kendo-angular-notification";
import { DatePipe } from "@angular/common";
import { TcareCampaignDialogMessageComponent } from "../tcare-campaign-dialog-message/tcare-campaign-dialog-message.component";
import { PartnerCategoryBasic } from "src/app/partner-categories/partner-category.service";
import { IntlService } from "@progress/kendo-angular-intl";
import * as xml2js from "xml2js";
import { TcareCampaignStartDialogComponent } from "../tcare-campaign-start-dialog/tcare-campaign-start-dialog.component";

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
declare var mxGraph: any;
declare var mxImage: any;
declare var mxPerimeter: any;
declare var mxConnectionHandler: any;
declare var mxEffects: any;
declare var mxAutoSaveManager: any;
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
  cellRule: any;
  doc: any;
  submited = false;

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
    this.load();
  }

  main(container, sidebar_sequences, sidebar_goals, model?) {
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
        new mxDivResizer(sidebar_sequences);
        new mxDivResizer(sidebar_goals);
      }

      //create obj
      var sequence = that.doc.createElement("sequence");
      var rule = that.doc.createElement("rule");
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

      // Centers the port icon on the cellSequence port
      graph.connectionHandler.cellSequenceConnectImage = true;

      // Does not allow dangling edges
      graph.setAllowDanglingEdges(false);

      // Sets the graph container and configures the editor
      editor.setGraphContainer(container);
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

      // var mgr = new mxAutoSaveManager(editor.graph, 1, 1);
      // if (mgr.isEnabled) {
      //   mgr.graphModelChanged = function () {
      //     that.saveChange(graph);
      //   };
      // }

      //validate Connection
      graph.isValidConnection = function (source, cellSequence) {
        const source_id = source.id;
        const cellSequence_id = cellSequence.id;
        var source_edge_length = 0;
        if (source.edges) {
          source_edge_length = source.edges.length;
        }

        for (let i = 0; i < source_edge_length; i++) {
          if (
            source_id == source.edges[i].source.id &&
            cellSequence_id == source.edges[i].cellSequence.id
          ) {
            return false;
          }
          if (
            source_id == source.edges[i].cellSequence.id &&
            cellSequence_id == source.edges[i].source.id
          ) {
            return false;
          }
        }
        var styleSource = this.getModel().getStyle(source);
        var stylecellSequence = this.getModel().getStyle(cellSequence);

        if (styleSource == stylecellSequence || !stylecellSequence)
          return false;
        if (styleSource == 'read' || styleSource == 'unread' || stylecellSequence == 'read' || stylecellSequence == 'unread')
          return false;

        if (styleSource == "sequence" && stylecellSequence == "rule")
          return false;
        else {
          var value = cellSequence.getValue();
          value.setAttribute('parent', source.id);
          graph.getModel().setValue(cellSequence, value);
          return true;
        }
      };

      //Add ContextMenu
      graph.popupMenuHandler.factoryMethod = function (menu, cell, evt) {
        return that.createPopupMenu(editor, graph, menu, cell, evt);
      };

      //defind NodeName
      graph.convertValueToString = function (cell) {
        if (mxUtils.isNode(cell.value)) {
          if (cell.value.nodeName.toLowerCase() == "rule") {
            return "Điều kiện";
          }

          if (cell.value.nodeName.toLowerCase() == "sequence") {
            return "Gửi tin";
          }

          if (cell.value.nodeName.toLowerCase() == "addtag") {
            return "Gán nhãn";
          }

          if (cell.value.nodeName.toLowerCase() == "messageopen") {
            return cell.value.getAttribute("label", "");
          }

          return cell.value.getAttribute("label", "");
        }

        return "";
      };

      //double click Action
      graph.dblClick = function (evt, cell) {
        if (this.isEnabled() && !mxEvent.isConsumed(evt) && cell != null && that.campaign.state != 'running') {
          if (cell.value.nodeName.toLowerCase() == 'sequence') {
            that.popupSequence(graph, cell);
          }

          if (cell.value.nodeName.toLowerCase() == "rule") {
            that.popupRule(graph, cell);
          }

          if (cell.value.nodeName.toLowerCase() == "addtag") {
            that.popupAddTag(graph, cell);
          }
        }
        else {
          that.notificationService.show({
            content: 'Kịch bản đang chạy bạn không thể thao tác, vui lòng dừng kịch bản để thao tác!',
            hideAfter: 3000,
            position: { horizontal: 'center', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'error', icon: true }
          });
          return false;
        }
        // Disables any default behaviour for the double click
        mxEvent.consume(evt);
      };

      // Enables new connections
      graph.setConnectable(true);

      // Adds all required styles to the graph (see below)
      that.configureStylesheet(graph);

      //Delete cell
      var keyHandler = new mxKeyHandler(graph);
      keyHandler.target;
      keyHandler.bindKey(46, function (evt) {
        if (graph.isEnabled) {
          if (that.campaign.state != 'running') {
            var cell = graph.getSelectionCell();
            if (cell.style == 'sequence' && cell.edges) {
              cell.edges.forEach(edge => {
                if (edge.value != '' && edge.value != null && edge.target) {
                  var cellTag = graph.getModel().getCell(edge.target.id);
                  if (cellTag) {
                    graph.getModel().remove(cellTag);
                    that.onSave()
                  }
                }
              });
            }
            graph.removeCells();
          }
          else {
            that.notificationService.show({
              content: 'Kịch bản đang chạy bạn không thể xóa!',
              hideAfter: 3000,
              position: { horizontal: 'center', vertical: 'top' },
              animation: { type: 'fade', duration: 400 },
              type: { style: 'error', icon: true }
            });
          }
        }
      });

      // thêm 1 sequence
      that.addSidebarIcon(
        graph,
        sidebar_sequences,
        sequence,
        "./assets/editors/images/message-setting.png",
        "sequence"
      );

      //thêm 1 rule
      that.addSidebarIcon(graph, sidebar_goals, rule, "./assets/editors/images/rule.png", "rule");

      //load Xml
      graph.getModel().beginUpdate();
      try {
        var doc = mxUtils.parseXml(model.graphXml);
        var dec = new mxCodec(doc);
        dec.decode(doc.documentElement, graph.getModel());
        graph.setSelectionCells(graph.getModel().cells);
      }
      finally {
        graph.getModel().endUpdate();
      }

    }
  }

  load() {
    this.tcareService.get(this.id).subscribe(
      result => {
        this.campaign = result;
        this.formGroup.get('name').patchValue(result.name);
        if (result.graphXml) {
          this.main(
            document.getElementById("graphContainer"),
            document.getElementById("sidebarContainer_sequences"),
            document.getElementById("sidebarContainer_goals"),
            result
          );
        } else {
          var value = {
            state: 'draf'
          }
          this.main(
            document.getElementById("graphContainer"),
            document.getElementById("sidebarContainer_sequences"),
            document.getElementById("sidebarContainer_goals"), value
          );
        }
      }
    )
  }

  addToolbarButton(editor, toolbar, action, label, image, isTransparent?) {
    var button = document.createElement("button");
    button.style.fontSize = "10";
    if (image != null) {
      var img = document.createElement("img");
      img.setAttribute("src", image);
      img.style.width = "16px";
      img.style.height = "16px";
      img.style.verticalAlign = "middle";
      img.style.marginRight = "2px";
      button.appendChild(img);
    }
    if (isTransparent) {
      button.style.background = "transparent";
      button.style.color = "#FFFFFF";
      button.style.border = "none";
    }
    mxEvent.addListener(button, "click", function (evt) {
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

      if (typeShape == "sequence") {
        var sequence_cells = model.filterCells(vertices, function (c) {
          return c.value.nodeName.toLowerCase() == "sequence";
        });

        if (sequence_cells.length) {
          that.notificationService.show({
            content: "Không thể kéo nhiều gửi tin",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "error", icon: true },
          });

          return false;
        }
      }

      if (typeShape == "rule") {
        var rule_cells = model.filterCells(vertices, function (c) {
          return c.value.nodeName.toLowerCase() == "rule";
        });

        if (rule_cells.length) {
          that.notificationService.show({
            content: "Không thể kéo nhiều điều kiện",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "error", icon: true },
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
        graph.setSelectionCell(v1);
      } finally {
        model.endUpdate();
        that.saveChange(graph)
      }

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
    var ds = mxUtils.makeDraggable(
      img,
      graph,
      funct,
      dragImage,
      0,
      0,
      true,
      true
    );
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
    style[mxConstants.STYLE_STROKECOLOR] = "#000000";
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] =
      that.base_url + "assets/editors/images/message-setting.png";
    style[mxConstants.STYLE_IMAGE_WIDTH] = "40";
    style[mxConstants.STYLE_IMAGE_HEIGHT] = "40";
    style[mxConstants.STYLE_SPACING_TOP] = "55";
    style[mxConstants.STYLE_SPACING] = "0";
    graph.getStylesheet().putCellStyle("sequence", style);

    //style for rule
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = "#000000";
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] =
      that.base_url + "assets/editors/images/rule.png";
    style[mxConstants.STYLE_IMAGE_WIDTH] = "40";
    style[mxConstants.STYLE_IMAGE_HEIGHT] = "40";
    style[mxConstants.STYLE_SPACING_TOP] = "55";
    style[mxConstants.STYLE_SPACING] = "-8";
    graph.getStylesheet().putCellStyle("rule", style);

    //style for message-read
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = "#000000";
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] =
      that.base_url + "assets/editors/images/message-tag-open.png";
    style[mxConstants.STYLE_IMAGE_WIDTH] = "35";
    style[mxConstants.STYLE_IMAGE_HEIGHT] = "35";
    style[mxConstants.STYLE_SPACING_TOP] = "50";
    style[mxConstants.STYLE_SPACING] = "-2.5";
    graph.getStylesheet().putCellStyle("read", style);

    //style for message-unread
    style = mxUtils.clone(style);
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_LABEL;
    style[mxConstants.STYLE_STROKECOLOR] = "#000000";
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_IMAGE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_IMAGE] =
      that.base_url + "assets/editors/images/message-tag.png";
    style[mxConstants.STYLE_IMAGE_WIDTH] = "35";
    style[mxConstants.STYLE_IMAGE_HEIGHT] = "35";
    style[mxConstants.STYLE_SPACING_TOP] = "50";
    style[mxConstants.STYLE_SPACING] = "-2.5";
    graph.getStylesheet().putCellStyle("unread", style);
  }

  popupAddTag(graph, cell) {
    let that = this;
    var value = cell.getValue();

    var serializer = new XMLSerializer();
    var valueXMl = serializer.serializeToString(value);

    xml2js.parseString(valueXMl, function (err, result) {
      if (!err) {
        var addTag = result.addTag;
        var item = new Object();
        item["tags"] = [];

        if (addTag.tag) {
          addTag.tag.forEach((t) => {
            item["tags"].push(Object.assign({}, t.$));
          });
        }

        let modalRef = that.modalService.open(
          TcareCampaignDialogMessageComponent,
          {
            size: "sm",
            windowClass: "o_technical_modal",
            scrollable: true,
            backdrop: "static",
            keyboard: false,
          }
        );
        modalRef.componentInstance.item = item;

        modalRef.result.then(
          (result: any) => {
            graph.getModel().beginUpdate();
            try {
              var doc = mxUtils.createXmlDocument();
              var userObject = doc.createElement("addTag");

              result.tags.forEach((t) => {
                var tagEl = doc.createElement("tag");
                for (var p in t) {
                  if (p == "id" || p == "name") {
                    tagEl.setAttribute(p, t[p]);
                  }
                }

                userObject.appendChild(tagEl);
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
          rule.condition.forEach((con) => {
            a.conditions.push(Object.assign({}, con.$));
          });
        }
        let modalRef = that.modalService.open(
          TcareCampaignDialogRuleComponent,
          {
            size: "lg",
            windowClass: "o_technical_modal",
            backdrop: "static",
            keyboard: false,
          }
        );
        modalRef.componentInstance.title = "Cài đặt điều kiện";
        modalRef.componentInstance.audience_filter = a;
        modalRef.result.then(
          (result: any) => {
            graph.getModel().beginUpdate();
            try {
              var doc = mxUtils.createXmlDocument();
              var userObject = doc.createElement("rule");
              for (var p in result) {
                if (p != "conditions") {
                  userObject.setAttribute(p, result[p]);
                }
              }

              result.conditions.forEach((con) => {
                var conEl = doc.createElement("condition");
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

    let modalRef = that.modalService.open(
      TcareCampaignDialogSequencesComponent,
      {
        size: "lg",
        windowClass: "o_technical_modal",
        scrollable: true,
        backdrop: "static",
        keyboard: false,
      }
    );
    modalRef.componentInstance.title = "Cài đặt gửi tin";
    modalRef.componentInstance.model = objx;

    modalRef.result.then(
      (result: any) => {
        graph.getModel().beginUpdate();
        try {
          var userObject = that.doc.createElement("sequence");
          userObject.setAttribute("campaignId", that.id);
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
      if (cell.style == "sequence") {
        menu.addItem(
          "Gán nhãn đã đọc",
          "./assets/editors/images/icons/message-tag-open-icon.png",
          function () {
            //Nếu có edge messageOpen từ cell này thì sẽ ko cho add thêm nữa
            var model = graph.getModel();
            //tìm tất cả edge của root
            var all_edges = model.filterDescendants(function (c) {
              return graph.model.isEdge(c);
            });

            //lọc edge kết nối vơi cell (source là cellId)
            var connect_edges = model.filterCells(all_edges, function (c) {
              return (
                c.source.id == cell.id &&
                c.value.nodeName.toLowerCase() == "messageopen"
              );
            });

            if (connect_edges.length) {
              that.notificationService.show({
                content: "Đã thêm gán nhãn đã đọc",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "error", icon: true },
              });
            } else {
              graph.getModel().beginUpdate();
              try {
                var addTagEl = that.doc.createElement("addTag");
                addTagEl.setAttribute("label", "Gán nhãn");

                var addTagVertex = graph.insertVertex(
                  parent,
                  null,
                  addTagEl,
                  cell.geometry.x + 200,
                  cell.geometry.y - 50,
                  40,
                  40,
                  "read"
                );

                var messageOpenEl = that.doc.createElement('messageOpen');
                messageOpenEl.setAttribute('label', 'Đã đọc');
                messageOpenEl.setAttribute('name', 'message_open');
                graph.insertEdge(parent, null, messageOpenEl, cell, addTagVertex);
                graph.setSelectionCell(addTagVertex);
              }
              finally {
                graph.getModel().endUpdate();
                that.saveChange(graph);
              }

            }
          });

        menu.addItem(
          "Gán nhãn đã nhận",
          "./assets/editors/images/icons/message-tag-icon.png",
          function () {
            //Nếu có edge messageDelivered từ cell này thì sẽ ko cho add thêm nữa
            var model = graph.getModel();
            var all_edges = model.filterDescendants(function (c) {
              return graph.model.isEdge(c);
            });

            var connect_edges = model.filterCells(all_edges, function (c) {
              return (
                c.source.id == cell.id &&
                c.value.nodeName.toLowerCase() == "messagedelivered"
              );
            });

            if (connect_edges.length) {
              that.notificationService.show({
                content: "Đã thêm gán nhãn đã nhận",
                hideAfter: 3000,
                position: { horizontal: "center", vertical: "top" },
                animation: { type: "fade", duration: 400 },
                type: { style: "error", icon: true },
              });
            } else {
              graph.getModel().beginUpdate();
              try {
                var addTagEl = that.doc.createElement("addTag");
                addTagEl.setAttribute("label", "Gán nhãn");

                var addTagVertex = graph.insertVertex(
                  parent,
                  null,
                  addTagEl,
                  cell.geometry.x + 200,
                  cell.geometry.y + 50,
                  40,
                  40,
                  "unread"
                );

                var messageDeliveredEl = that.doc.createElement('messageDelivered');
                messageDeliveredEl.setAttribute('label', 'Đã nhận');
                messageDeliveredEl.setAttribute('name', 'message_delivered');
                graph.insertEdge(parent, null, messageDeliveredEl, cell, addTagVertex);
                graph.setSelectionCell(addTagVertex);
              }
              finally {
                graph.getModel().endUpdate();
              }

            }
          })
      }
    } else {
      menu.addItem("Fit", "./assets/editors/images/zoom.gif", function () {
        graph.fit();
      });
      menu.addItem(
        "Actual",
        "./assets/editors/images/zoomactual.gif",
        function () {
          graph.zoomActual();
        }
      );
    }
  }

  checkConnection() {
    var listCell = [];
    var graph = this.editorDefind.graph;
    var cells = graph.getModel().cells;
    for (let index = 0; index < graph.getModel().nextId + 1; index++) {
      if (cells[index])
        listCell.push(cells[index]);
    }
    var cellRule = listCell.find(x => x.style == "rule");
    if (cellRule) {
      var edge = listCell.find(x => (x.source && x.source.id) === cellRule.id);
      if (edge) {
        var cellSequence = listCell.find(x => x.id == (edge.target && edge.target.id));
        if (cellSequence)
          return true;
        else
          return false;
      } else { return false; }
    } else { return false; }
  }

  startCampaign() {
    if (!this.checkConnection()) {
      this.notificationService.show({
        content: 'Bạn chưa hoàn thành 1 kịch bản, vui lòng hoàn thành sau đó chạy lại kịch bản!.',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'error', icon: true }
      });
      return false;
    }
    let modalRef = this.modalService.open(TcareCampaignStartDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', backdrop: 'static', keyboard: false });
    modalRef.componentInstance.title = 'Cài đặt thời gian chạy kịch bản';
    modalRef.result.then((result: any) => {
      if (result) {
        result.id = this.id;
        result.sheduleStart = this.intlService.formatDate(
          result.sheduleStart,
          "yyyy-MM-ddTHH:mm:ss"
        );
        this.campaign.sheduleStart = result.sheduleStart;
        this.tcareService.actionStartCampaign(result).subscribe(() => {
          this.campaign.state = "running";
          this.notificationService.show({
            content: "Chạy kịch bản thành công!.",
            hideAfter: 3000,
            position: { horizontal: "center", vertical: "top" },
            animation: { type: "fade", duration: 400 },
            type: { style: "success", icon: true },
          });
        });
      }
    });
  }

  stopCampaign() {
    if (this.campaign.id) {
      var value = [];
      value.push(this.campaign.id);
      this.tcareService.actionStopCampaign(value).subscribe(() => {
        this.campaign.state = "stopped";
        this.notificationService.show({
          content: "Dừng kịch bản thành công!.",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
      });
    }
  }

  saveChange(graph) {
    var value = this.formGroup.value;
    var enc = new mxCodec(mxUtils.createXmlDocument());
    var node = enc.encode(graph.getModel());
    value.graphXml = mxUtils.getPrettyXml(node);
    if (this.id) {
      this.tcareService.update(this.id, value).subscribe(() => { });
    }
  }

  onSave() {
    this.submited = true;
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
          content: "Thành công",
          hideAfter: 3000,
          position: { horizontal: "center", vertical: "top" },
          animation: { type: "fade", duration: 400 },
          type: { style: "success", icon: true },
        });
      });
    }
  }

  get nameControl() {
    return this.formGroup.get("name");
  }
}
