import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsAccountListComponent } from './sms-account-list.component';

describe('SmsAccountListComponent', () => {
  let component: SmsAccountListComponent;
  let fixture: ComponentFixture<SmsAccountListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsAccountListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsAccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
