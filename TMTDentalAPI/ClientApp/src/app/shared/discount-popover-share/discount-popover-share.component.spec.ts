import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DiscountPopoverShareComponent } from './discount-popover-share.component';

describe('DiscountPopoverShareComponent', () => {
  let component: DiscountPopoverShareComponent;
  let fixture: ComponentFixture<DiscountPopoverShareComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DiscountPopoverShareComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DiscountPopoverShareComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
