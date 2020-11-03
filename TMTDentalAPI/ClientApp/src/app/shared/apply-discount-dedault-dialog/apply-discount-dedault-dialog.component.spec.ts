import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplyDiscountDedaultDialogComponent } from './apply-discount-dedault-dialog.component';

describe('ApplyDiscountDedaultDialogComponent', () => {
  let component: ApplyDiscountDedaultDialogComponent;
  let fixture: ComponentFixture<ApplyDiscountDedaultDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplyDiscountDedaultDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplyDiscountDedaultDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
