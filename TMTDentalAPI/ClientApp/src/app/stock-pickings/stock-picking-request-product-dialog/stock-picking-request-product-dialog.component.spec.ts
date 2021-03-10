import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingRequestProductDialogComponent } from './stock-picking-request-product-dialog.component';

describe('StockPickingRequestProductDialogComponent', () => {
  let component: StockPickingRequestProductDialogComponent;
  let fixture: ComponentFixture<StockPickingRequestProductDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingRequestProductDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingRequestProductDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
