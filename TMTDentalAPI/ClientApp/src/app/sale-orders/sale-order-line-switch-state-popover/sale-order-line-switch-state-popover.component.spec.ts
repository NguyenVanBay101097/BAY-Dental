import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineSwitchStatePopoverComponent } from './sale-order-line-switch-state-popover.component';

describe('SaleOrderLineSwitchStatePopoverComponent', () => {
  let component: SaleOrderLineSwitchStatePopoverComponent;
  let fixture: ComponentFixture<SaleOrderLineSwitchStatePopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineSwitchStatePopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineSwitchStatePopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
