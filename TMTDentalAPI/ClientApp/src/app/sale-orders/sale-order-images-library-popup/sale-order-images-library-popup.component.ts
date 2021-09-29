import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
@Component({
  selector: 'app-sale-order-images-library-popup',
  templateUrl: './sale-order-images-library-popup.component.html',
  styleUrls: ['./sale-order-images-library-popup.component.css']
})
export class SaleOrderImagesLibraryPopupComponent implements OnInit {

  images: any = [];
  imagesGroup: any[] = [];
  imagesSelected: any[] = [];
  search: string ='';
  searchUpdate = new Subject<string>();
  data: any[] =[];
  items: any[] = [];
  id: string = '';
  constructor(public activeModal: NgbActiveModal,private intlService: IntlService,
    private saleOrderService: SaleOrderService
    ) { }
  
  ngOnInit() {
    this.loadImages();
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe((val) => {
        var items = this.images.filter((img,index)=> {
          return img.name.toUpperCase().indexOf(val) != -1
        });
        this.groupByImages(items);
      });
  }

  loadImages(){
    this.saleOrderService.getListAttachment(this.id).subscribe(res => {
      this.images = res;
      this.groupByImages(res);
    })
  }

  onChoose(){
    this.activeModal.close(this.imagesSelected);
  }

  onCancel(){
    this.activeModal.close([]);
  }

  chooseImage(img){
    if (this.imagesSelected.length == 0){
      this.imagesSelected.push(img);
      return;
    }
      
    var index = this.imagesSelected.findIndex(x => x.id == img.id);
    if (index != -1)
      this.imagesSelected.splice(index,1);
    else
      this.imagesSelected.push(img);
  }

  isSelected(img){
    var item = this.imagesSelected.find(x => x.id == img.id);
    if (item)
      return true;
    return false;
  }

  groupByImages(items:any){
    let data = null;
    data = items.reduce((r, a) => {
      const dateFormat = moment(a.dateCreated).format('YYYY-MM-DD');
      r[dateFormat] = r[dateFormat] || [];
      r[dateFormat].push(a);
      return r;
    }, Object.create(null));

    this.imagesGroup = Object.keys(data).map( (key)=> [key, data[key]]);
  }

  getTitleDate(dateStr) {
    const yesterday = moment().subtract(1, 'days').format('YYYY-MM-DD');
    const today = moment().format('YYYY-MM-DD');
    switch (dateStr) {
      case yesterday:
        return 'Hôm qua';
      case today:
        return 'Hôm nay';
      default:
        return moment(new Date(dateStr)).locale('vi').format('LL');
    }
  }
}
