import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderCreateDotKhamDialogComponent } from './sale-order-create-dot-kham-dialog.component';

describe('SaleOrderCreateDotKhamDialogComponent', () => {
  let component: SaleOrderCreateDotKhamDialogComponent;
  let fixture: ComponentFixture<SaleOrderCreateDotKhamDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderCreateDotKhamDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderCreateDotKhamDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
