import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingCreateUpdateComponent } from './stock-picking-create-update.component';

describe('StockPickingCreateUpdateComponent', () => {
  let component: StockPickingCreateUpdateComponent;
  let fixture: ComponentFixture<StockPickingCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
