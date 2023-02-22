import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingMlDialogComponent } from './stock-picking-ml-dialog.component';

describe('StockPickingMlDialogComponent', () => {
  let component: StockPickingMlDialogComponent;
  let fixture: ComponentFixture<StockPickingMlDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingMlDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingMlDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
