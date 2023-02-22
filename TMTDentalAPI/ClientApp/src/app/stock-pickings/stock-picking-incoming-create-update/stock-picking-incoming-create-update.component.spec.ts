import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingIncomingCreateUpdateComponent } from './stock-picking-incoming-create-update.component';

describe('StockPickingIncomingCreateUpdateComponent', () => {
  let component: StockPickingIncomingCreateUpdateComponent;
  let fixture: ComponentFixture<StockPickingIncomingCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingIncomingCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingIncomingCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
