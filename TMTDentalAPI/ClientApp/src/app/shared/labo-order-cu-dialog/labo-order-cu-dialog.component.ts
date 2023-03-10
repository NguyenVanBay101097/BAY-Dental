import { Component, OnInit, ViewChild } from '@angular/core';
import { AsyncValidatorFn } from '@angular/forms';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as _ from 'lodash';
import { Observable, of, timer } from 'rxjs';
import { debounceTime, map, switchMap, tap } from 'rxjs/operators';
import { AuthService } from 'src/app/auth/auth.service';
import { SessionInfoStorageService } from 'src/app/core/services/session-info-storage.service';
import { WebService } from 'src/app/core/services/web.service';
import { LaboBiteJointBasic, LaboBiteJointService } from 'src/app/labo-bite-joints/labo-bite-joint.service';
import { LaboBridgeBasic, LaboBridgePageSimple, LaboBridgeService } from 'src/app/labo-bridges/labo-bridge.service';
import { LaboFinishLineBasic, LaboFinishLinePageSimple, LaboFinishLineService } from 'src/app/labo-finish-lines/labo-finish-line.service';
import { LaboImageBasic, LaboOrderDefaultGet, LaboOrderDisplay, LaboOrderService } from 'src/app/labo-orders/labo-order.service';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerImageBasic, PartnerService } from 'src/app/partners/partner.service';
import { ProductSimple } from 'src/app/products/product-simple';
import { ProductFilter, ProductService } from 'src/app/products/product.service';
import { ToothBasic, ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic } from 'src/app/tooth-categories/tooth-category.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';
import { ImageViewerComponent } from '../image-viewer/image-viewer.component';
import { PartnerSupplierCuDialogComponent } from '../partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { PrintService } from '../services/print.service';
import { IrAttachmentBasic } from '../shared';

@Component({
  selector: 'app-labo-order-cu-dialog',
  templateUrl: './labo-order-cu-dialog.component.html',
  styleUrls: ['./labo-order-cu-dialog.component.css']
})
export class LaboOrderCuDialogComponent implements OnInit {
  @ViewChild('partnerCbx', { static: true }) partnerCbx: ComboBoxComponent;
  title: string;
  myForm: FormGroup;
  id: string;// c?? th??? l?? input
  saleOrderLineId: string; // c?? th??? l?? input
  laboOrder: LaboOrderDisplay = new LaboOrderDisplay();
  // saleOrderLineLabo: any; // l?? input

  partners: any = [];
  labos: ProductSimple[] = [];
  finishlines: LaboFinishLineBasic[] = [];
  biteJoints: LaboBiteJointBasic[] = [];
  bridges: LaboBridgeBasic[] = [];
  attachs: ProductSimple[] = [];
  listType = {
    'up_right': [],
    'up_left': [],
    'down_right': [],
    'down_left': []
  };
  submitted = false;
  constructor(private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private modalService: NgbModal,
    private notificationService: NotificationService,
    private partnerService: PartnerService,
    private laboOrderService: LaboOrderService,
    private intlService: IntlService,
    private productService: ProductService,
    private finishLineService: LaboFinishLineService,
    private biteJointService: LaboBiteJointService,
    private bridgeService: LaboBridgeService,
    private webService: WebService,
    private printService: PrintService,
    private router: Router,
    private toothService: ToothService,
    private sessionInfoStorageService: SessionInfoStorageService,
    private authService: AuthService
    
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      state: 'draft',
      partner: [null, Validators.required],
      dateOrderObj: [null, Validators.required],
      datePlannedObj: null,
      dateReceiptObj: null,
      teeth: this.fb.array([]),
      color: null,
      quantity: 1,
      priceUnit: 0,
      amountTotal: 0,
      indicated: null,
      note: null,
      warrantyCode: [null, Validators.compose([]), this.validateWarrantyCode.bind(this)],
      warrantyPeriodObj: null,
      productId: [null],
      product: [null],
      laboFinishLineId: [null],
      laboFinishLine: [null],
      laboBiteJointId: null,
      laboBiteJoint: null,
      laboBridgeId: null,
      laboBridge: null,
      laboOrderProducts: this.fb.array([]),//attach
      technicalNote: null,
      images: this.fb.array([]),
      saleOrderLineId: [null, Validators.required],
    });

    this.partnerCbx.filterChange.asObservable().pipe(
      debounceTime(300),
      tap(() => (this.partnerCbx.loading = true)),
      switchMap(value => this.searchPartners(value))
    ).subscribe(result => {
      this.partners = result;
      this.partnerCbx.loading = false;
    });
    this.loadPartners();

    if (this.id) {
      this.loadData();
    } else {
      this.loadDefault();
    }
    this.loadCategory();


  }

  get state() { return this.myForm.get('state').value; }
  get dateOrderObjFC() { return this.myForm.get('dateOrderObj'); }
  get datePlannedObjFC() { return this.myForm.get('datePlannedObj'); }
  get warrantyPeriodObjFC() { return this.myForm.get('warrantyPeriodObj'); }
  get warrantyCodeFC() { return this.myForm.get('warrantyCode'); }
  get saleOrderLine() { return this.laboOrder.saleOrderLine; }
  get teethFA() { return this.myForm.get('teeth') as FormArray; }
  get teeth() { return this.myForm.get('teeth').value; }
  get laboOrderProductsFA() { return this.myForm.get('laboOrderProducts') as FormArray; }
  get laboOrderProducts() { return this.myForm.get('laboOrderProducts').value; }
  get imagesFA() { return this.myForm.get('images') as FormArray; }
  get quantityFC() { return this.myForm.get('quantity'); }
  get priceUnitFC() { return this.myForm.get('priceUnit'); }
  get amountTotalFC() { return this.myForm.get('amountTotal'); }
  get partnerFC() { return this.myForm.get('partner'); }
  get indicatedFC() { return this.myForm.get('indicated'); }
  get noteFC() { return this.myForm.get('note'); }
  get technicalNoteFC() { return this.myForm.get('technicalNote'); }
  get productId() {return this.myForm.get('productId').value};
  get laboFinishLineId() {return this.myForm.get('laboFinishLineId').value};
  get laboBiteJointId() {return this.myForm.get('laboBiteJointId').value};
  get laboBridgeId() {return this.myForm.get('laboBridgeId').value};
  get dateReceiptObj() {return this.myForm.get('dateReceiptObj')};
  validateWarrantyCode(
    control: AbstractControl
  ): Observable<ValidationErrors | null> {
    const val = control.value;
    if (!val || (val && val.trim() == '')) {
      return of(null);
    }
    return timer(500).pipe(
      switchMap(() =>
        this.laboOrderService.checkExistWarrantyCode({ code: control.value, id: this.id }).pipe(
          map(ex => {
            if (ex == false) return null;
            return ({ exist: true })
          })
        )
      )
    );
  }

  notify(style, content) {
    this.notificationService.show({
      content: content,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

  searchPartners(filter?: string) {
    var val = new PartnerPaged();
    val.supplier = true;
    val.search = filter;
    if (this.sessionInfoStorageService.getSessionInfo().settings && !this.sessionInfoStorageService.getSessionInfo().settings.companySharePartner) {
      val.companyId = this.authService.userInfo.companyId;
    }
    return this.partnerService.getAutocompleteSimple(val);
  }

  loadPartners() {
    this.searchPartners().subscribe(result => {
      this.partners = _.unionBy(this.partners, result, 'id');
    });
  }

  patchValue(res: LaboOrderDisplay) {
    this.myForm.patchValue(res);
    let dateOrder = new Date(res.dateOrder);
    this.dateOrderObjFC.patchValue(dateOrder);

    if (res.datePlanned) {
      let datePlanned = this.intlService.parseDate(res.datePlanned);
      this.datePlannedObjFC.patchValue(datePlanned);
    }
    if (res.dateReceipt){
      let dateReceipt = this.intlService.parseDate(res.dateReceipt);
      this.dateReceiptObj.patchValue(dateReceipt);
    }

    if (res.warrantyPeriod) {
      let warrantyPeriod = this.intlService.parseDate(res.warrantyPeriod);
      this.warrantyPeriodObjFC.patchValue(warrantyPeriod);
    }

    if (res.partner) {
      this.partners = _.unionBy(this.partners, [res.partner], 'id');
    }
    //patch teeth
    if (res.teeth) {
      this.teethFA.clear();
      res.teeth.forEach(tooth => {
        this.teethFA.push(this.fb.group(tooth));
      });
    }
    // patch attach
    if (res.laboOrderProducts) {
      this.laboOrderProductsFA.clear();
      res.laboOrderProducts.forEach(p => {
        this.laboOrderProductsFA.push(this.fb.group(p));
      });
    }
    //patch images
    if (res.images) {
      this.imagesFA.clear();
      res.images.forEach(img => {
        this.imagesFA.push(this.fb.group(img));
      });
    }

  }

  loadData() {
    this.laboOrderService.get(this.id).subscribe(result => {
      this.laboOrder = result;
      this.patchValue(result);
      if (result.saleOrderLine.teeth){
        this.processTeeth(result.saleOrderLine.teeth);
      }
    });
  }

  loadDefault() {
    var df = new LaboOrderDefaultGet();
    df.saleOrderLineId = this.saleOrderLineId;
    this.laboOrderService.defaultGet(df).subscribe(result => {
      this.laboOrder = result;
      if (result.saleOrderLine.teeth){
        this.processTeeth(result.saleOrderLine.teeth);
      }
      result.quantity = 1;
      this.patchValue(result);
      (result.saleOrderLine && result.saleOrderLine.product) ? this.priceUnitFC.patchValue(result.saleOrderLine.product.laboPrice) : '';
    });
  }

  loadCategory() {
    //load labo
    const laboPaged = new ProductFilter();
    laboPaged.limit = 1000;
    laboPaged.type2 = 'labo';
    this.productService.autocomplete2(laboPaged).subscribe(res => {
      this.labos = res;
      
    });
    //load labofinishline
    const finishPaged = new LaboFinishLinePageSimple();
    this.finishLineService.autoComplete(finishPaged).subscribe((res: any) => {
      this.finishlines = res;
      
    });
    //load bitejoint
    const biteJointPaged = new LaboFinishLinePageSimple();
    this.biteJointService.autoComplete(biteJointPaged).subscribe((res: any) => {
      this.biteJoints = res;

    });
    //load bridge
    const bridgePaged = new LaboBridgePageSimple();
    this.bridgeService.autoComplete(bridgePaged).subscribe((res: any) => {
      this.bridges = res;
    });
    //load attach
    const attachPaged = new ProductFilter();
    attachPaged.limit = 1000;
    attachPaged.type2 = 'labo_attach';
    this.productService.autocomplete2(attachPaged).subscribe(res => {
      this.attachs = res;
    });
  }

  isToothSelected(tooth: ToothDisplay) {
    const index = this.teeth.findIndex(x => x.id == tooth.id);
    return index >= 0 ? true : false;
  }

  onToothSelected(tooth: ToothDisplay) {
    if (this.isToothSelected(tooth)) {
      var index = this.teeth.findIndex(x => x.id == tooth.id);
      this.teethFA.removeAt(index);
    } else {
      this.teethFA.push(this.fb.group(tooth));
    }

    //update quantity combobox
    this.quantityFC.setValue(this.teeth.length);
  }


  getAmountTotal() {
    return this.priceUnitFC.value * this.quantityFC.value;
  }

  getItem(arr: any[],id:any){
    var item = arr.find(x => x.id == id);
    if (item)
      return item.name;
    return '';
  }

  isAttachSelected(attach: ProductSimple) {
    const index = this.laboOrderProducts.findIndex(x => x.id == attach.id);
    return index >= 0 ? true : false;
  }

  onAttachSelected(attach: ProductSimple) {
    if (this.isAttachSelected(attach)) {
      var index = this.laboOrderProducts.findIndex(x => x.id == attach.id);
      this.laboOrderProductsFA.removeAt(index);
    } else {
      this.laboOrderProductsFA.push(this.fb.group(attach));
    }
  }

  stopPropagation(e) {
    e.stopPropagation();
  }

  onViewImg(imgObj: IrAttachmentBasic) {
    const modalRef = this.modalService.open(ImageViewerComponent, { windowClass: 'o_image_viewer o_modal_fullscreen' });
    const img = {
      id: imgObj.id,
      name: imgObj.name,
      date: null,
      note: null,
      url: imgObj.url
    };
    const imgs = this.imagesFA.value.map(x => {
      return {
        id: x.id,
        name: x.name,
        date: null,
        note: null,
        url: x.url
      };
    });
    modalRef.componentInstance.images = imgs;
    modalRef.componentInstance.selectedImage = img;
  }

  onRemoveImg(i) {
    this.imagesFA.removeAt(i);
  }

  onFileChange(e) {
    const file_node = e.target;
    const formData = new FormData();
    for (var i = 0; i < file_node.files.length; i++) {
      formData.append('files', file_node.files[i]);
    }

    this.webService.uploadImages(formData).subscribe((res: any) => {
      res.forEach(img => {
        const imgObj = new IrAttachmentBasic();
        imgObj.name = img.fileName;
        imgObj.url = img.fileUrl;
        const imgFG = this.fb.group(imgObj);
        this.imagesFA.push(imgFG);
      });
    });
  }

  onSave$(): Observable<any> {
    const val = this.myForm.value;
    val.dateOrder = this.intlService.formatDate(val.dateOrderObj, 'yyyy-MM-ddTHH:mm:ss');
    val.datePlanned = val.datePlannedObj ? this.intlService.formatDate(val.datePlannedObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.warrantyPeriod = val.warrantyPeriodObj ? this.intlService.formatDate(val.warrantyPeriodObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    val.partnerId = val.partner.id;
    val.dateReceipt = val.dateReceiptObj ? this.intlService.formatDate(val.dateReceiptObj, 'yyyy-MM-ddTHH:mm:ss') : null;
    // val.productId = val.product ? val.product.id : null;
    // val.laboBridgeId = val.laboBridge ? val.laboBridge.id : null;
    // val.laboBiteJointId = val.laboBiteJoint ? val.laboBiteJoint.id : null;
    // val.laboFinishLineId = val.laboFinishLine ? val.laboFinishLine.id : null;
    val.amountTotal = this.getAmountTotal();

    if (this.id) {
      return this.laboOrderService.update(this.id, val);
    } else {
      return this.laboOrderService.create(val);
    }
  }

  onSave() {
    this.submitted = true;
    
    if (this.myForm.invalid) {
      return;
    }

    this.onSave$().subscribe((res: any) => {
      if (this.id) {
        this.notify('success', 'L??u th??nh c??ng');
        // this.loadData();
        this.activeModal.close();
      } else {
        this.notify('success', 'T???o th??nh c??ng');
        this.id = res.id;
        // this.loadData();
        this.activeModal.close(res);
      }
    });
  }

  onConfirm() {
    if (this.myForm.invalid) {
      return;
    }

    this.onSave$().subscribe((res: any) => {
      if (!this.id) {
        this.id = res.id;
      }
      this.laboOrderService.buttonConfirm([this.id]).subscribe(() => {
        this.activeModal.close();
        this.notify('success', '?????t h??ng th??nh c??ng');
      });
    });
  }

  onCancel() {
    this.laboOrderService.buttonCancel([this.id]).subscribe(() => {
      this.notify('success', 'H???y phi???u th??nh c??ng');
      this.activeModal.close(true);
    });
  }

  onClose() {
    if (this.id) {
      this.activeModal.close(true);
    } else {
      this.activeModal.close();
    }
  }

  onQuickCreatePartner() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Th??m: Nh?? cung c???p';

    modalRef.result.then(result => {
      var p = new PartnerSimple();
      p.id = result.id;
      p.name = result.name;
      p.displayName = result.displayName;
      this.partnerFC.patchValue(p);
      this.partners = _.unionBy(this.partners, [p], 'id');
    }, () => {
    });
  }

  onQuickUpdatePartner() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'S???a: Nh?? cung c???p';
    modalRef.componentInstance.id = this.partnerFC.value.id;

    modalRef.result.then(() => {
    }, () => {
    });
  }

  processTeeth(teeth: ToothDisplay[]) {
    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.viTriHam == '0_up' && tooth.position == '0_right')
        this.listType.up_right.push(tooth);
      if (tooth.viTriHam == '0_up' && tooth.position == '1_left')
        this.listType.up_left.push(tooth);
      if (tooth.viTriHam == '1_down' && tooth.position == '0_right')
        this.listType.down_right.push(tooth);
      if (tooth.viTriHam == '1_down' && tooth.position == '1_left')
        this.listType.down_left.push(tooth);
    }
  }

  printLaboOrder() {
    if (this.id) {
      this.laboOrderService.getPrint(this.id).subscribe((result: any) => {
        this.printService.printHtml(result.html);
      })
    }
  }

  actionRedirect(saleOrderLine){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(() => {
      this.router.navigate(['/sale-orders/', saleOrderLine?.orderId]);
    });
    this.activeModal.dismiss();
  }
}
