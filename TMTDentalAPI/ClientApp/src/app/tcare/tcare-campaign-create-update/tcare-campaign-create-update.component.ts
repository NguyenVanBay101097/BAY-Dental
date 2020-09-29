import { Component, OnInit, Inject, Renderer2, Input, Output, EventEmitter, OnChanges, SimpleChanges, ViewChild } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import {
  TcareService,
  TCareCampaignDisplay,
} from "../tcare.service";
import { TcareCampaignDialogSequencesComponent } from "../tcare-campaign-dialog-sequences/tcare-campaign-dialog-sequences.component";
import { TcareCampaignDialogRuleComponent } from "../tcare-campaign-dialog-rule/tcare-campaign-dialog-rule.component";

import { NotificationService } from "@progress/kendo-angular-notification";
import { TcareCampaignDialogMessageComponent } from "../tcare-campaign-dialog-message/tcare-campaign-dialog-message.component";
import { IntlService, load } from "@progress/kendo-angular-intl";
import * as xml2js from "xml2js";
import { TcareCampaignStartDialogComponent } from "../tcare-campaign-start-dialog/tcare-campaign-start-dialog.component";
import { mxgraph } from 'src/mxgraph-types';
import { PartnerCategoryBasic, PartnerCategoryPaged, PartnerCategoryService } from 'src/app/partner-categories/partner-category.service';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { debounceTime, switchMap, tap } from 'rxjs/operators';
import { result } from 'lodash';
import { PartnerCategoryCuDialogComponent } from 'src/app/partner-categories/partner-category-cu-dialog/partner-category-cu-dialog.component';

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
export class TcareCampaignCreateUpdateComponent implements OnInit, OnChanges {
  @ViewChild('tagCbx', { static: true }) tagCbx: ComboBoxComponent;
  @Input() campaign: TCareCampaignDisplay;
  @Output('actionNext') actionNext = new EventEmitter<any>();
  @Output('timeChange') timeChange = new EventEmitter<any>();
  filterdTags: PartnerCategoryBasic[] = [];
  offsetY = 30;
  formCampaign: FormGroup;
  priority = null;
  title = "Kịch bản";
  one = true;
  editor: any;
  graph: any;
  cellRule: any;
  doc: any;
  disableTime = false;
  submited = false;
  constructor(
    private fb: FormBuilder,
    @Inject("BASE_API") private base_url: string,
    private modalService: NgbModal,
    private tcareService: TcareService,
    private renderer2: Renderer2,
    private notificationService: NotificationService,
    private intlService: IntlService,
    private tagService: PartnerCategoryService
  ) {
    this.editor = new mxEditor();
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.formCampaign = this.fb.group({
      // name: ["", Validators.required],
      scheduleStart: null,
      tagId: null
    });

    this.tagCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.tagCbx.loading = true)),
      switchMap(value => this.searchTags(value))
    ).subscribe(result => {
      this.filterdTags = result;
      this.tagCbx.loading = false;
    });

    var scheduleStart = new Date(this.campaign.sheduleStart);
    this.formCampaign.patchValue({ scheduleStart });
    this.formCampaign.get('tagId').patchValue(this.campaign.tagId);
    this.load();
    this.loadTags();
  }

  ngOnInit() {
    this.formCampaign = this.fb.group({
      // name: ["", Validators.required],
      scheduleStart: null,
      tagId: null
    });

    var scheduleStart = new Date(this.campaign.sheduleStart);
    this.formCampaign.patchValue({ scheduleStart });
    this.formCampaign.get('tagId').patchValue(this.campaign.tagId);
    this.load();
  }

  quickAddTagDialog() {
    var modalRef = this.modalService.open(PartnerCategoryCuDialogComponent, { size: "sm", windowClass: "o_technical_modal", scrollable: true, backdrop: "static", keyboard: false, });
    modalRef.componentInstance.title = "Tạo mới nhãn";

    modalRef.result.then(
      result => {
        if (result) {
          this.loadTags();
          this.formCampaign.get('tagId').setValue(result.id);
        }
      }
    )
  }

  loadTags() {
    this.searchTags().subscribe(
      result => {
        this.filterdTags = result;
      }
    )
  }

  onChange(event) {
    this.timeChange.emit(event);
  }

  searchTags(q?: string) {
    var paged = new PartnerCategoryPaged();
    paged.limit = 20;
    paged.offset = 0;
    paged.search = q ? q : '';
    return this.tagService.autocomplete(paged);
  }

  main(container, sidebar_sequences, sidebar_goals, model) {
    var that = this;
    if (!mxClient.isBrowserSupported()) {
      // Displays an error message if the browser is not supported.
      mxUtils.error("Browser is not supported!", 200, false);
    } else {
      this.doc = mxUtils.createXmlDocument();
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



      that.editor.graph.setCellsMovable(true);
      that.editor.graph.setAutoSizeCells(true);
      that.editor.graph.setPanning(true);
      that.editor.graph.centerZoom = true;
      that.editor.graph.panningHandler.useLeftButtonForPanning = true;

      // Uses the port icon while connections are previewed
      mxConnectionHandler.prototype.connectImage = new mxImage(that.base_url + "assets/editors/images/connector.gif", 16, 16);

      // Centers the port icon on the cellSequence port
      that.editor.graph.connectionHandler.cellSequenceConnectImage = true;

      // Does not allow dangling edges
      that.editor.graph.setAllowDanglingEdges(false);

      // Sets the graph container and configures the editor
      that.editor.setGraphContainer(container);

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
      that.editor.graph.isValidConnection = function (source, cellSequence) {
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
          that.editor.graph.getModel().setValue(cellSequence, value);
          return true;
        }

      };

      //Add ContextMenu
      that.editor.graph.popupMenuHandler.factoryMethod = function (menu, cell, evt) {
        return that.createPopupMenu(that.editor, that.editor.graph, menu, cell, evt);
      };

      //defind NodeName
      that.editor.graph.convertValueToString = function (cell) {
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
      that.editor.graph.dblClick = function (evt, cell) {
        if (this.isEnabled() && !mxEvent.isConsumed(evt) && !that.campaign.active) {
          if (cell.value.nodeName.toLowerCase() == 'sequence') {
            that.popupSequence(that.editor.graph, cell);
          }

          if (cell.value.nodeName.toLowerCase() == "rule") {
            that.popupRule(that.editor.graph, cell);
          }

          if (cell.value.nodeName.toLowerCase() == "addtag") {
            that.popupAddTag(that.editor.graph, cell);
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
      that.editor.graph.setConnectable(true);

      // Adds all required styles to the graph (see below)
      that.configureStylesheet(that.editor.graph);

      //Delete cell
      var keyHandler = new mxKeyHandler(that.editor.graph);
      keyHandler.target;
      keyHandler.bindKey(46, function (evt) {
        if (that.editor.graph.isEnabled) {
          if (that.campaign.state != 'running') {

            var cell = that.editor.graph.getSelectionCell();
            if (cell.style == 'sequence' && cell.edges) {
              cell.edges.forEach(edge => {
                if (edge.value != '' && edge.value != null && edge.target) {
                  var cellTag = that.editor.graph.getModel().getCell(edge.target.id);
                  if (cellTag) {
                    that.editor.graph.getModel().remove(cellTag);
                  }
                }
              });
            }
            that.editor.graph.removeCells();

            // that.onSave();
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

      if (this.one) {
        // thêm 1 sequence
        that.addSidebarIcon(that.editor.graph, sidebar_sequences, sequence, "./assets/editors/images/message-setting.png", "sequence");

        //thêm 1 rule
        that.addSidebarIcon(that.editor.graph, sidebar_goals, rule, "./assets/editors/images/rule.png", "rule");
      }

      //load Xml    
      that.editor.graph.getModel().beginUpdate();
      try {
        for (let i = 2; i < that.editor.graph.getModel().nextId + 4; i++) {
          if (that.editor.graph.getModel().cells[i])
            that.editor.graph.getModel().remove(that.editor.graph.getModel().cells[i]);
        }
        var doc = mxUtils.parseXml(model.graphXml);
        var dec = new mxCodec(doc);
        dec.decode(doc.documentElement, that.editor.graph.getModel());
        // that.editor.graph.setSelectionCells(that.editor.graph.getModel().cells);
      }
      finally {
        that.editor.graph.getModel().endUpdate();
      }

      // var mgr = new mxAutoSaveManager(that.editor.graph);
      // mxAutoSaveManager.prototype.autoSaveDelay = 1;
      // mxAutoSaveManager.prototype.autoSaveThreshold = 1;
      // mgr.save = function()
      // {
      //   console.log('save');
      //   that.onSave();
      // };

      // mgr.graphModelChanged = function(	changes	) {
      //   console.log(changes);
      // }
    }
  }

  load() {
    // this.formCampaign.get('name').patchValue(this.campaign.name);
    // this.formCampaign.get('sheduleStart').patchValue(new Date(this.campaign.sheduleStart));
    if (this.campaign.graphXml) {
      this.main(
        document.getElementById("graphContainer"),
        document.getElementById("sidebarContainer_sequences"),
        document.getElementById("sidebarContainer_goals"),
        this.campaign
      );
    } else {
      var value = {
        state: 'draf'
      }
      this.main(
        document.getElementById("graphContainer"),
        document.getElementById("sidebarContainer_sequences"),
        document.getElementById("sidebarContainer_goals"),
        value
      );
    }
    this.one = false;
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
        // that.onSave()
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

        let modalRef = that.modalService.open(TcareCampaignDialogMessageComponent, { size: "sm", windowClass: "o_technical_modal", scrollable: true, backdrop: "static", keyboard: false, });
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
              // that.onSave();
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
        let modalRef = that.modalService.open(TcareCampaignDialogRuleComponent, { size: "lg", windowClass: "o_technical_modal", backdrop: "static", keyboard: false, });
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
              // that.onSave();
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
    let modalRef = that.modalService.open(TcareCampaignDialogSequencesComponent, { size: "lg", windowClass: "o_technical_modal", scrollable: true, backdrop: "static", keyboard: false });
    modalRef.componentInstance.title = "Cài đặt gửi tin";
    modalRef.componentInstance.model = objx;

    modalRef.result.then(
      (result: any) => {
        graph.getModel().beginUpdate();
        try {
          var userObject = that.doc.createElement("sequence");
          userObject.setAttribute("campaignId", that.campaign.id);
          for (var p in result) {
            userObject.setAttribute(p, result[p]);
          }

          graph.getModel().setValue(cell, userObject);
        } finally {
          graph.getModel().endUpdate();
          // that.onSave();
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
                var addTagVertex = graph.insertVertex(parent, null, addTagEl, cell.geometry.x + 200, cell.geometry.y - 50, 40, 40, "read");
                var messageOpenEl = that.doc.createElement('messageOpen');
                messageOpenEl.setAttribute('label', 'Đã đọc');
                messageOpenEl.setAttribute('name', 'message_open');
                graph.insertEdge(parent, null, messageOpenEl, cell, addTagVertex);
                graph.setSelectionCell(addTagVertex);
              }
              finally {
                graph.getModel().endUpdate();
                // that.onSave();
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
                var addTagVertex = graph.insertVertex(parent, null, addTagEl, cell.geometry.x + 200, cell.geometry.y + 50, 40, 40, "unread");
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
    var graph = this.editor.graph;
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

  get sheduleStartControl() { return this.formCampaign.get('sheduleStart') }

  onChangeTimeStartCampaign() {
    var scheduleStartControl = this.formCampaign.get('scheduleStart');
    var value = {
      id: this.campaign ? this.campaign.id : null,
      sheduleStart: scheduleStartControl ? this.intlService.formatDate(scheduleStartControl.value, "yyyy-MM-ddTHH:mm:ss") : null
    };

    this.tcareService.actionSetSheduleStartCampaign(value).subscribe(
      () => {
        this.actionNext.emit({ sheduleStart: value.sheduleStart });
        console.log(this.campaign);
        // this.notificationService.show({
        //   content: 'Cài thời gian chạy chiến dịch thành công !',
        //   hideAfter: 3000,
        //   position: { horizontal: 'center', vertical: 'top' },
        //   animation: { type: 'fade', duration: 400 },
        //   type: { style: 'success', icon: true }
        // });
      }
    )

    // if (!this.checkConnection()) {
    //   this.formCampaign.get('sheduleStart').patchValue(new Date());
    //   this.notificationService.show({
    //     content: 'Bạn chưa hoàn thành 1 kịch bản, vui lòng hoàn thành sau đó chạy lại kịch bản!.',
    //     hideAfter: 3000,
    //     position: { horizontal: 'center', vertical: 'top' },
    //     animation: { type: 'fade', duration: 400 },
    //     type: { style: 'error', icon: true }
    //   });
    //   return false;
    // } else {
    //   var value = {
    //     id: this.campaign ? this.campaign.id : null,
    //     sheduleStart: this.intlService.formatDate(this.formCampaign.get('sheduleStart').value, "yyyy-MM-ddTHH:mm:ss")
    //   }
    //   this.tcareService.actionSetSheduleStartCampaign(value).subscribe(
    //     () => {
    //       this.actionNext.emit(null);
    //       this.notificationService.show({
    //         content: 'Cài thời gian chạy chiến dịch thành công !',
    //         hideAfter: 3000,
    //         position: { horizontal: 'center', vertical: 'top' },
    //         animation: { type: 'fade', duration: 400 },
    //         type: { style: 'success', icon: true }
    //       });
    //     }
    //   )
    // }

  }

  onSave() {
    // this.submited = true;
    // if (this.formCampaign.invalid) {
    //   return false;
    // }

    var value = this.formCampaign.value;
    this.campaign.sheduleStart = value.scheduleStart ? this.intlService.formatDate(value.scheduleStart, "yyyy-MM-ddTHH:mm:ss") : null;
    var enc = new mxCodec(mxUtils.createXmlDocument());
    var node = enc.encode(this.editor.graph.getModel());
    this.campaign.tagId = value.tagId;
    this.campaign.graphXml = mxUtils.getPrettyXml(node);

    this.tcareService.update(this.campaign.id, this.campaign).subscribe(() => {
      this.actionNext.emit(this.campaign);
      this.notificationService.show({
        content: "Lưu chiến dịch thành công",
        hideAfter: 3000,
        position: { horizontal: "center", vertical: "top" },
        animation: { type: "fade", duration: 400 },
        type: { style: "success", icon: true },
      });
    });
  }

  get nameControl() {
    return this.formCampaign.get("name");
  }
}
