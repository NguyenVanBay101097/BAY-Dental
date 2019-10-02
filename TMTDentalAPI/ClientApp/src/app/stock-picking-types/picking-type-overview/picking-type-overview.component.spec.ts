import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PickingTypeOverviewComponent } from './picking-type-overview.component';

describe('PickingTypeOverviewComponent', () => {
  let component: PickingTypeOverviewComponent;
  let fixture: ComponentFixture<PickingTypeOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PickingTypeOverviewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PickingTypeOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
