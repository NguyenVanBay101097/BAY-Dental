import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockPickingOutgoingCreateUpdateComponent } from './stock-picking-outgoing-create-update.component';

describe('StockPickingOutgoingCreateUpdateComponent', () => {
  let component: StockPickingOutgoingCreateUpdateComponent;
  let fixture: ComponentFixture<StockPickingOutgoingCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockPickingOutgoingCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockPickingOutgoingCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
