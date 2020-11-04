import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TcareMessagingListComponent } from './tcare-messaging-list.component';

describe('TcareMessagingListComponent', () => {
  let component: TcareMessagingListComponent;
  let fixture: ComponentFixture<TcareMessagingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TcareMessagingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TcareMessagingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
