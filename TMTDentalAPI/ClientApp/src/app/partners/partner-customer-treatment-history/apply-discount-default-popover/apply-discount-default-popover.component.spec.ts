import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyDiscountDefaultPopoverComponent } from './apply-discount-default-popover.component';

describe('ApplyDiscountDefaultPopoverComponent', () => {
  let component: ApplyDiscountDefaultPopoverComponent;
  let fixture: ComponentFixture<ApplyDiscountDefaultPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplyDiscountDefaultPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplyDiscountDefaultPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
