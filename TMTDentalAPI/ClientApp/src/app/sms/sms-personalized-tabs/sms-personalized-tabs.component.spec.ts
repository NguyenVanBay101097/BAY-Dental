import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SmsPersonalizedTabsComponent } from './sms-personalized-tabs.component';

describe('SmsPersonalizedTabsComponent', () => {
  let component: SmsPersonalizedTabsComponent;
  let fixture: ComponentFixture<SmsPersonalizedTabsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SmsPersonalizedTabsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SmsPersonalizedTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
