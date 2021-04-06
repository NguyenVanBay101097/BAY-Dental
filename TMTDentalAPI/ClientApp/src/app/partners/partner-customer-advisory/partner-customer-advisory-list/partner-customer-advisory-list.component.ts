import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { NotificationService } from '@progress/kendo-angular-notification';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { AdvisoryPaged, AdvisoryService, AdvisoryToothAdvise, CreateFromAdvisoryInput } from 'src/app/advisories/advisory.service';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';
import { PrintService } from 'src/app/shared/services/print.service';
import { ToothDisplay, ToothFilter, ToothService } from 'src/app/teeth/tooth.service';
import { ToothCategoryBasic, ToothCategoryService } from 'src/app/tooth-categories/tooth-category.service';
import { ToothDiagnosisService } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';
import { PartnerCustomerAdvisoryCuDialogComponent } from '../partner-customer-advisory-cu-dialog/partner-customer-advisory-cu-dialog.component';

@Component({
  selector: 'app-partner-customer-advisory-list',
  templateUrl: './partner-customer-advisory-list.component.html',
  styleUrls: ['./partner-customer-advisory-list.component.css']
})
export class PartnerCustomerAdvisoryListComponent implements OnInit {

  searchUpdate = new Subject<string>();
  search: string;
  public monthStart: Date = new Date(new Date(new Date().setDate(1)).toDateString());
  public monthEnd: Date = new Date(new Date(new Date().setDate(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0).getDate())).toDateString());
  dateFrom: Date;
  dateTo: Date;
  hamList: { [key: string]: {} };
  teethSelected: any[] = [];
  teethConsulted: string[] = [];
  filteredToothCategories: any[] = [];
  cateId: string;
  gridData: GridDataResult;
  limit: number = 10;
  skip: number = 0;
  loading = false;
  customerId: string;
  mySelection = [];
  
  constructor(
    private modalService: NgbModal,
    private toothService: ToothService,
    private toothCategoryService: ToothCategoryService,
    private advisoryService: AdvisoryService,
    private activeRoute: ActivatedRoute,
    private intlService: IntlService,
    private notificationService: NotificationService,
    private printService: PrintService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.dateFrom = this.monthStart;
    this.dateTo = this.monthEnd;
    this.loadToothCategories();
    setTimeout(() => {
      this.loadDefaultToothCategory().subscribe(result => {
        this.cateId = result.id;
        this.loadTeethMap(result);
      })
      this.loadTeethConsulted();
    }, 200);

    this.activeRoute.parent.params.subscribe(
      params => {
        this.customerId = params.id;
      }
    )
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((value) => {
        this.search = value || '';
        this.skip = 0;
        this.loadDataFromApi();
      });
    this.loadDataFromApi();
    
  }
  createAdvisory(){
    const modalRef = this.modalService.open(PartnerCustomerAdvisoryCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm thông tin tư vấn';
    modalRef.componentInstance.customerId = this.customerId;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { })
  }

  createQuotations(){
    if (!this.mySelection || this.mySelection.length == 0) 
    {
      this.notify('error', 'Bạn chưa chọn thông tin tư vấn');
      return;
    }
    var val = new CreateFromAdvisoryInput();
    val.customerId = this.customerId;
    val.ids = this.mySelection;
    this.advisoryService.createQuotations(val).subscribe(
      result => {
        this.router.navigate(['/quotations/form'], {
          queryParams: {
            id: result.id
          },
        });
      }
    )
  }

  createSaleOrder(){
    if (!this.mySelection || this.mySelection.length == 0) 
    {
      this.notify('error', 'Bạn chưa chọn thông tin tư vấn');
      return;
    }
    var val = new CreateFromAdvisoryInput();
    val.customerId = this.customerId;
    val.ids = this.mySelection;
    this.advisoryService.createSaleOrder(val).subscribe(
      result => {
        this.router.navigate(['/sale-orders/form'], {
          queryParams: {
            id: result.id
          },
        });
      }
    )
  }

  loadTeethMap(categ: ToothCategoryBasic) {
    var val = new ToothFilter();
    val.categoryId = categ.id;
    return this.toothService.getAllBasic(val).subscribe(result => this.processTeeth(result));
  }

  processTeeth(teeth: ToothDisplay[]) {
    this.hamList = {
      '0_up': { '0_right': [], '1_left': [] },
      '1_down': { '0_right': [], '1_left': [] }
    };

    for (var i = 0; i < teeth.length; i++) {
      var tooth = teeth[i];
      if (tooth.position === '1_left') {
        this.hamList[tooth.viTriHam][tooth.position].push(tooth);
      } else {
        this.hamList[tooth.viTriHam][tooth.position].unshift(tooth);
      }
    }
  }

  isSelected(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return true;
      }
    }

    return false;
  }

  isConsulted(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethConsulted.length; i++) {
      if (this.teethConsulted[i] === tooth.id) {
        return true;
      }
    }

    return false;
  }



  onSelected(tooth: ToothDisplay) {
    if (this.isSelected(tooth)) {
      var index = this.getSelectedIndex(tooth);
      this.teethSelected.splice(index, 1);
    } else {
      this.teethSelected.push(tooth);
    }
    this.loadDataFromApi();
  }

  reSelected(){
    this.teethSelected = [];
    this.loadDataFromApi();
  }

  getSelectedIndex(tooth: ToothDisplay) {
    for (var i = 0; i < this.teethSelected.length; i++) {
      if (this.teethSelected[i].id === tooth.id) {
        return i;
      }
    }

    return null;
  }

  loadDefaultToothCategory() {
    return this.toothCategoryService.getDefaultCategory();
  }

  loadToothCategories() {
    return this.toothCategoryService.getAll().subscribe(result => this.filteredToothCategories = result);
  }

  onChangeToothCategory(value: any) {
    if (value.id) {
      this.teethSelected = [];
      this.loadTeethMap(value);
      this.cateId = value.id;
      this.loadDataFromApi();
    }
  }

  loadTeethConsulted() {
    var val = new AdvisoryToothAdvise();
    val.customerId = this.customerId;
    this.advisoryService.getToothAdvise(val).subscribe((result:any) => {
      this.teethConsulted = result.toothIds;
    })
  }

  loadDataFromApi(){
    var val = new AdvisoryPaged();
    this.loading = true;
    val.limit = this.limit;
    val.search = this.search ? this.search : "";
    val.offset = this.skip;
    val.dateFrom = this.dateFrom? this.intlService.formatDate(this.dateFrom,"yyyy-MM-dd") : "";
    val.dateTo = this.dateTo? this.intlService.formatDate(this.dateTo,"yyyy-MM-dd") : "";
    val.customerId = this.customerId;
    val.toothIds = this.teethSelected.map(x => x.id);
    this.advisoryService.getPaged(val).subscribe(res => {
      this.gridData =  <GridDataResult> {
        data: res.items,       
        total: res.totalItems
      };
      this.loading = false;
    })
  }

  searchChangeDate(data){
    this.dateTo = data.dateTo;
    this.dateFrom = data.dateFrom;
    this.skip = 0;
    this.loadDataFromApi();
  }

  pageChange(event:PageChangeEvent){
    this.skip = event.skip;
    this.loadDataFromApi();
  }

  getTeeth(teeth){
    if(teeth)
      return teeth.map(x => x.name).join(',');
    return null;
  }

  getToothDiagnosis(toothDiagnosis){
    if(toothDiagnosis)
      return toothDiagnosis.map(x => x.name).join(',');
    return null;
  }

  getProducts(products){
    if(products)
      return products.map(x => x.name).join(',');
    return null;
  }

  editItem(data){
    const modalRef = this.modalService.open(PartnerCustomerAdvisoryCuDialogComponent, { scrollable: true, size: 'xl', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa thông tin tư vấn';
    modalRef.componentInstance.customerId = this.customerId;
    modalRef.componentInstance.id = data.id;
    modalRef.result.then(() => {
      this.loadDataFromApi();
    }, er => { })
  }

  deleteItem(data){
    let modalRef = this.modalService.open(ConfirmDialogComponent, {
      windowClass: "o_technical_modal",
      keyboard: false,
      backdrop: "static",
    });
    modalRef.componentInstance.body = "Bạn có chắc chắn xóa tư vấn?";
    modalRef.result.then(() => {
      this.advisoryService.remove(data.id).subscribe(() => {
        this.notificationService.show({
          content: 'Xóa thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        this.loadDataFromApi();
      }, (err) => {
        console.log(err);
      });
    }, () => { });
  }

  notify(style, text) {
    this.notificationService.show({
      content: text,
      hideAfter: 3000,
      position: { horizontal: 'center', vertical: 'top' },
      animation: { type: 'fade', duration: 400 },
      type: { style: style, icon: true }
    });
  }

    onPrint() {
    if(!this.customerId) return;
    if (!this.mySelection || this.mySelection.length == 0) 
    {
      this.notify('error', 'Bạn chưa chọn thông tin tư vấn');
      return;
    }
    this.advisoryService.getPrint(this.customerId,this.mySelection).subscribe((res: any) => {
      this.printService.printHtml(res.html);
    });
  }

}
