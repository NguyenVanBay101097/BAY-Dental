import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { FacebookTagsPaged, FacebookTagsService } from '../facebook-tags.service';
import { map, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NotificationService } from '@progress/kendo-angular-notification';

@Component({
  selector: 'app-facebook-select-tag',
  templateUrl: './facebook-select-tag.component.html',
  styleUrls: ['./facebook-select-tag.component.css']
})
export class FacebookSelectTagComponent implements OnInit {
  @Input() selectedTags_receive: any;
  @Output() selectedTags_send = new EventEmitter<any>();

  listTags: any[];
  selectedTags: any[] = [];
  searchTag: string;
  searchTagUpdate = new Subject<string>();

  constructor(private facebookTagsService: FacebookTagsService, 
    private notificationService: NotificationService) { }

  ngOnInit() {
    this.selectedTags = this.selectedTags_receive;

    setTimeout(() => {
      this.loadListTags();
    })

    this.searchTagUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadListTags();
      });
  }

  loadListTags() {
    var val = new FacebookTagsPaged();
    val.offset = 0;
    val.limit = 10;
    val.search = this.searchTag || '';
    this.facebookTagsService.getTags(val).subscribe(res => {
      this.listTags = res['items'];
      // console.log(this.listTags);
    }, err => {
      console.log(err);
    })
  }

  valueNormalizer = (name$: Observable<string>) => name$.pipe(map((name: string) => {
    //search for matching item to avoid duplicates

    //search in values
    const matchingValue: any = this.selectedTags.find((item: any) => {
      return item["name"] === name;
    });

    if (matchingValue) {
      return matchingValue;
    }

    //search in data
    const matchingItem: any = this.listTags.find((item: any) => {
      return item["name"] === name;
    });

    if (matchingItem) {
      return matchingItem;
    } else {
      this.createTag(name);
      return {
        id: null, //generate unique value for the custom item
        name: name
      };
    }
  }));

  createTag(name) {
    var val = {
      name: name
    };
    this.facebookTagsService.create(val).subscribe((res: any) => {
      this.notificationService.show({
        content: 'Tạo nhãn thành công',
        hideAfter: 3000,
        position: { horizontal: 'center', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      // console.log(res);
      this.selectedTags.find(tag => tag.name === name).id = res.id; // Add id tag to selectedTags
    }, err => {
      console.log(err);
    })
  }

  filterChangeTag(event) {
    this.searchTag = event;
  }

  valueChangeTag() {
    this.selectedTags_send.emit(this.selectedTags);
  }
}
