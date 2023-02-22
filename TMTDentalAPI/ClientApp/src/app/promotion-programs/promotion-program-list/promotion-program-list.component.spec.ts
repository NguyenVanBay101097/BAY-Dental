import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PromotionProgramListComponent } from './promotion-program-list.component';

describe('PromotionProgramListComponent', () => {
  let component: PromotionProgramListComponent;
  let fixture: ComponentFixture<PromotionProgramListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PromotionProgramListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PromotionProgramListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
